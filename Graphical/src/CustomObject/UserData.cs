using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphical
{
    public class UserData : Dictionary<string, object>
    {
        public UserData() : base() { }

        /// <summary>
        /// Returns the value associated with the key
        /// if exists in the UserData and is of the given Type.
        /// Returns the default value of the given Type otherwise.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetValueOrDefault<T>(string key)
        {
            object value;

            if(!this.TryGetValue(key, out value) || typeof(T) != value.GetType())
            {
                return default(T);
            }

            return (T)value;

        }
    }
}
