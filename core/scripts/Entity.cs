using System;
using UnityEngine;

namespace educa.core
{
    [System.Serializable]
    public class Entity
    {
        private Guid id;
        private string name;
        private string description;
        private GameObject gameObject;

        public Entity()
        {
            id = Guid.NewGuid();
        }

        public Entity(string name, string description)
        {
            id = Guid.NewGuid();
            this.name = name;
            this.description = description;
        }

        public Guid Id { get => id; }
        public string Name { get => name; set => name = value; }
        public string Description { get => description; set => description = value; }
        public GameObject GameObject { get => gameObject; set => gameObject = value; }
    }
}

