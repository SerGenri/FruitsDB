using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

//https://archive.codeplex.com/?p=kindofmagic

namespace FruitsDB.DB.Base
{

    public class MagicAttribute : Attribute { }
    public class NoMagicAttribute : Attribute { }

    [Magic]
    public abstract class BaseRaisePropertyChanged : INotifyPropertyChanged
    {

        /// <summary>
        /// Принудительный зов PropertyChanged в нужном месте
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)] // to preserve method call 
        protected static void Raise() { }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void RaisePropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
