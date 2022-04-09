using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BigRookGames.Weapons
{
    public class ProjectileController : MonoBehaviour
    {
        // --- Config ---
        public PlayerManager pm;
        public GameObject owner;
        public GameObject target;
        public float speed = 100;
        public int damage;

        GameObject gameManager;

        public LayerMask collisionLayerMask;

        // --- Explosion VFX ---
        public GameObject rocketExplosion;

        // --- Projectile Mesh ---
        public MeshRenderer projectileMesh;

        // --- Script Variables ---
        private bool targetHit;

        // --- Audio ---
        public AudioSource inFlightAudioSource;

        // --- VFX ---
        public ParticleSystem disableOnHit;

        Vector3 newPosition;

        private void Start()
        {
            gameManager = GameObject.Find("GameManager");
        }


        private void Update()
        {
            // --- Check to see if the target has been hit. We don't want to update the position if the target was hit ---
            if (targetHit) return;

            // --- moves the game object in the forward direction at the defined speed ---
            //transform.position += transform.forward * (speed * Time.deltaTime);
            if(target == null)
            {
                Destroy(gameObject);
                return;
            }
            UnitManager um = target.GetComponent<UnitManager>();
            BuildingManager bm = target.GetComponent<BuildingManager>();
            if(um != null && !um.IsAlive())
            {
                Destroy(gameObject);
                return;
            }
            else if(bm != null && !bm.IsAlive)
            {
                Destroy(gameObject);
                return;
            }
            Vector3 targetPos = target.transform.position;
            targetPos.y = targetPos.y + 5.0f;
            Vector3 direction = Vector3.Normalize(targetPos - transform.position);
            transform.forward = direction;
            newPosition = transform.position + speed * direction * Time.deltaTime;
            transform.position = newPosition;

        }


        /// <summary>
        /// Explodes on contact.
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerEnter(Collider other)
        {
            // --- return if not enabled because OnCollision is still called if compoenent is disabled ---
            if (!enabled) return;

            if (target != null && other.transform.parent.gameObject != null && target == other.transform.parent.gameObject)
            {
                // --- Explode when hitting an object and disable the projectile mesh ---
                Explode();
                projectileMesh.enabled = false;
                targetHit = true;
                inFlightAudioSource.Stop();
                foreach(Collider col in GetComponents<Collider>())
                {
                    col.enabled = false;
                }
                disableOnHit.Stop();
                DealDamage();
                // --- Destroy this object after 2 seconds. Using a delay because the particle system needs to finish ---
                Destroy(gameObject, 5f);
            }

        }
        public void DealDamage()
        {
            if (target != null && target.tag == "Unit")
            {
                UnitManager targetManager = target.GetComponent<UnitManager>();
                bool killed = targetManager.OnDamage(owner, damage);
                if (killed)
                {
                    GameSetting gameSetting = gameManager.GetComponent<GameSetting>();
                    int bounty = (int)(gameSetting.UnitPrice[(int)targetManager.pm.hero_t, (int)targetManager.type] * gameSetting.bountyRate);
                    if(pm != null) pm.AddGold(bounty);
                }
            }
            if (target != null && target.tag == "Building")
            {
                BuildingManager targetManager = target.GetComponent<BuildingManager>();
                bool destroyed = target.GetComponent<BuildingManager>().OnDamage(owner, damage);
                if (destroyed)
                {
                    GameSetting gameSetting = gameManager.GetComponent<GameSetting>();
                    int bounty = (int)(gameSetting.BuildingPrice[targetManager.side, (int)targetManager.type] * gameSetting.bountyRate);
                    if (pm != null) pm.AddGold(bounty);
                }
            }
        }

        /// <summary>
        /// Instantiates an explode object.
        /// </summary>
        private void Explode()
        {
            // --- Instantiate new explosion option. I would recommend using an object pool ---
            GameObject newExplosion = Instantiate(rocketExplosion, transform.position, rocketExplosion.transform.rotation, null);


        }
    }
}