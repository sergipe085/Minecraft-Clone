using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minecraft.WorldGeneration
{
    public class ChunkMB : MonoBehaviour
    {
        private Chunk owner = null;

        public void SetOwner(Chunk _owner) {
            owner = _owner;
        }

        private void SaveProgress() {
             if (owner.changed) {
                 owner.Save();
                 owner.changed = false;
             }
        }

        private void OnApplicationQuit() {
            SaveProgress();
        }
    }
}
