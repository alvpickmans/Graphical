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
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            object value;
            this.TryGetValue(key, out value);

            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));

            return (T)converter.ConvertFrom(value);

        }
    }
}
