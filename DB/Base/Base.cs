using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;

namespace FruitsDB.DB.Base
{
   
    public class Base : BaseRaisePropertyChanged, IEnumerable
    {
        // Реализуем интерфейс IEnumerable
        public virtual IEnumerator GetEnumerator()
        {
            yield return this;
        }
       
    }
}
