using MLAPI;
using System.Collections.Generic;
using UnityEngine;

namespace BossRoom.Visual
{

    /// <summary>
    /// The visual part of a TrampleAction. See TrampleAction.cs for more about this action type.
    /// 
    /// TrampleActionFX can include a visual "cue" object which is placed at the attacker's feet.
    /// If used, the object should auto-destroy itself after a fixed amount of time.
    /// 
    /// Note: unlike most ActionFX, this is NOT responsible for triggering hit-react animations on
    /// the trampled victims. The TrampleAction triggers these directly when it determines a collision.
    /// </summary>
    public class TrampleActionFX : ActionFX
    {
        public TrampleActionFX(ref ActionRequestData data, ClientCharacterVisualization parent) : base(ref data, parent) { }

        /// <summary>
        /// We spawn the "visual cue" graphics a moment after we begin our action.
        /// (A little extra delay helps ensure we have the correct orientation for the
        /// character, so the graphics are oriented in the right direction!)
        /// </summary>
        private const float k_GraphicsSpawnDelay = 0.3f;

        /// <summary>
        /// Prior to spawning graphics, this is null. Once we spawn the graphics, this is a list of everything we spawned.
        /// </summary>
        /// <remarks>
        /// Mobile performance note: constantly creating new GameObjects like this has bad performance on mobile and should
        /// be replaced with object-pooling (i.e. reusing the same art GameObjects repeatedly). But that's outside the scope of this demo.
        /// </remarks>
        private List<GameObject> m_SpawnedGraphics = null;

        public override bool Start()
        {
            m_Parent.OurAnimator.SetTrigger(Description.Anim);
            return true;
        }

        public override bool Update()
        {
            float age = Time.time - TimeStarted;
            if (age > k_GraphicsSpawnDelay && m_SpawnedGraphics == null)
            {
                m_SpawnedGraphics = new List<GameObject>();
                foreach (var go in Description.Spawns)
                {
                    GameObject cueGraphicsGO = GameObject.Instantiate(go, m_Parent.Parent.position, m_Parent.Parent.rotation, null);
                    m_SpawnedGraphics.Add(cueGraphicsGO);
                }
            }
            return true;
        }

        public override void Cancel()
        {
            // we've been aborted -- destroy the "cue graphics"
            if (m_SpawnedGraphics != null)
            {
                foreach (var go in m_SpawnedGraphics)
                {
                    if (go)
                    {
                        GameObject.Destroy(go);
                    }
                }
            }
            m_SpawnedGraphics = null;
        }

        public override void End()
        {
            // under normal circumstances, we allow the "cue graphics" to destroy themselves, so do nothing here
        }
    }
}
