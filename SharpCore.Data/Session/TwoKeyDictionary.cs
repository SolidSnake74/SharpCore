using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SharpCore.Data
{
    [Serializable]
    public class TwoKeyDictionary<PK, SK, V> : IEnumerable<KeyValuePair<PK, Dictionary<SK, V>>>
    {
        private Dictionary<PK, Dictionary<SK, V>> dic_pk;

        public TwoKeyDictionary()
        {
            this.dic_pk = new Dictionary<PK, Dictionary<SK, V>>();
        }

        public bool ContainsPrimaryKey(PK pk)
        {
            return this.dic_pk.ContainsKey(pk);
        }

        public bool ContainsKey(PK pk, SK sk)
        {
            V v;
            bool haskey = this.TryGetValue(pk, sk, out v);
            return haskey;
        }

        public bool TryGetValue(PK pk, SK sk, out V v)
        {
            Dictionary<SK, V> sk_dic;
            bool has_pk = this.dic_pk.TryGetValue(pk, out sk_dic);
            if (!has_pk)
            {
                v = default(V);
                return false;
            }
            
            return sk_dic.TryGetValue(sk, out v);
        }

        public V GetValue(PK pk, SK sk)
        {
            V v;
            bool haskey = this.TryGetValue(pk, sk, out v);
            if (!haskey)
            {
                string msg = string.Format("(pk,sk) missing");
                throw new KeyNotFoundException(msg);
            }
            
            return v;
        }

        public Dictionary<SK, V> GetValue(PK pk)
        {            

            Dictionary<SK, V> sk_dic;
            bool has_pk = this.dic_pk.TryGetValue(pk, out sk_dic);
            if (!has_pk)
            {                
                
            }

            return sk_dic;
        }

        public void SetValue(PK pk, SK sk, V v)
        {
            Dictionary<SK, V> sk_dic;
            bool has_pk = this.dic_pk.TryGetValue(pk, out sk_dic);
            if (!has_pk)
            {
                sk_dic = new Dictionary<SK, V>();
                this.dic_pk[pk] = sk_dic;
            }

            sk_dic[sk] = v;
        }

        public V this[PK pk, SK sk]
        {
            get
            {
                return this.GetValue(pk, sk);
            }

            set
            {
                this.SetValue(pk, sk, value);
            }
        }

        public Dictionary<SK, V> this[PK pk]
        {
            get
            {
                return this.GetValue(pk);
            }

            //set
            //{
            //    this.SetValue(pk, value);
            //}
        }

        public int Count
        {
            get
            {
                int n = 0;
                foreach (var i in this.dic_pk.Values)
                {
                    n += i.Count;
                }

                return n;
            }
        }

        public bool Remove(PK key)
        {
            return this.dic_pk.Remove(key);
        }

        public bool Remove(PK key,SK sk)
        {
            return this.dic_pk[key].Remove(sk);
        }

        public void Clear()
        {
            this.dic_pk.Clear();
        }

        public IEnumerator<KeyValuePair<PK, Dictionary<SK, V>>> GetEnumerator()
        {
            return this.dic_pk.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.dic_pk.GetEnumerator();
        }
    }
}
