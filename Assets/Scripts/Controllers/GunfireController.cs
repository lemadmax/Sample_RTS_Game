using UnityEngine;

namespace BigRookGames.Weapons
{
    public class GunfireController : MonoBehaviour
    {
        // --- Audio ---
        public AudioClip GunShotClip;
        public AudioSource source;
        public Vector2 audioPitch = new Vector2(.9f, 1.1f);

        public GameObject muzzlePrefab;

        public float CDTime = 2.0f;
        float CDTimer = 0.0f;

        // --- Projectile ---
        [Tooltip("The projectile gameobject to instantiate each time the weapon is fired.")]
        public GameObject projectilePrefab;

        BuildingManager bm;
        //PlayerManager pm;

        private void Start()
        {
            if (source != null) source.clip = GunShotClip;
            bm = GetComponentInParent<BuildingManager>();
            //pm = null;
        }

        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.S))
            {
                FireWeapon();
            }
            if(bm.target != null && bm.gameObject.tag != "Dead" && CDTimer <= 0.0f)
            {
                //Debug.Log(bm.target);
                FireWeapon();
                CDTimer = CDTime;
            }
            CDTimer = Mathf.Max(0.0f, CDTimer - Time.deltaTime);

        }

        /// <summary>
        /// Creates an instance of the muzzle flash.
        /// Also creates an instance of the audioSource so that multiple shots are not overlapped on the same audio source.
        /// Insert projectile code in this function.
        /// </summary>
        public void FireWeapon()
        {

            // --- Spawn muzzle flash ---
            var flash = Instantiate(muzzlePrefab, transform);

            // --- Shoot Projectile Object ---
            if (projectilePrefab != null)
            {
                GameObject newProjectile = Instantiate(projectilePrefab, transform.position, transform.rotation, transform);
                ProjectileController pc = newProjectile.GetComponent<ProjectileController>();
                pc.pm = null;
                pc.owner = gameObject;
                pc.target = bm.target;
                pc.damage = bm.damage;
            }

            // --- Handle Audio ---
            if (source != null)
            {
                // --- Sometimes the source is not attached to the weapon for easy instantiation on quick firing weapons like machineguns, 
                // so that each shot gets its own audio source, but sometimes it's fine to use just 1 source. We don't want to instantiate 
                // the parent gameobject or the program will get stuck in a loop, so we check to see if the source is a child object ---
                if(source.transform.IsChildOf(transform))
                {
                    source.Play();
                }
                else
                {
                    // --- Instantiate prefab for audio, delete after a few seconds ---
                    AudioSource newAS = Instantiate(source);
                    if ((newAS = Instantiate(source)) != null && newAS.outputAudioMixerGroup != null && newAS.outputAudioMixerGroup.audioMixer != null)
                    {
                        // --- Change pitch to give variation to repeated shots ---
                        newAS.outputAudioMixerGroup.audioMixer.SetFloat("Pitch", Random.Range(audioPitch.x, audioPitch.y));
                        newAS.pitch = Random.Range(audioPitch.x, audioPitch.y);

                        // --- Play the gunshot sound ---
                        newAS.PlayOneShot(GunShotClip);

                        // --- Remove after a few seconds. Test script only. When using in project I recommend using an object pool ---
                        Destroy(newAS.gameObject, 4);
                    }
                }
            }

            // --- Insert custom code here to shoot projectile or hitscan from weapon ---

        }

    }
}