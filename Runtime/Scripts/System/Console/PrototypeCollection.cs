using System.Collections;
using System.Collections.Generic;

namespace CGenStudios.UnityUtils
{
    public partial class Console
    {
        public class PrototypeCollection : IEnumerable<Prototype>
        {
            public PrototypeCollection() { }

            public Prototype this[string identifier] => Prototypes[identifier.ToLower()];

            private Dictionary<string, Prototype> Prototypes { get; } = new Dictionary<string, Prototype>();

            public int Count => Prototypes.Count;

            public bool Contains(string identifier)
            {
                return Prototypes.ContainsKey(identifier.ToLower());
            }

            public void Add(Prototype prototype)
            {
                Prototypes.Add(prototype.Identifier.ToLower(), prototype);
            }

            public void Remove(string identifier)
            {
                Prototypes.Remove(identifier.ToLower());
            }

            public void Remove(Prototype prototype)
            {
                Prototypes.Remove(prototype.Identifier.ToLower());
            }

            public void Clear()
            {
                Prototypes.Clear();
            }

            public IEnumerator<Prototype> GetEnumerator()
            {
                return Prototypes.Values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return Prototypes.Values.GetEnumerator();
            }
        }
    }
}
