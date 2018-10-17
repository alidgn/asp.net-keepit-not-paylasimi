using KeepIT.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeepIT.DataAccessLayer.EntityFramework
{
    public class RepositoryBase
    {
        private static DatabaseContext _db;
        private static object _LockControl = new object();

        protected RepositoryBase() // Protected Constructor ile bu sınıfın new'lenmesini engelleriz.
        {
            // Repository.cs sınıfında bu sınıfı kullanacağımız için newlenmesini engelleriz.
        }

        public static DatabaseContext CreateContext() // static oluşturarak newlenmesi engellenmiş bir sınıftaki metodların kullanımı sağlanır.
        {
            if (_db == null) // Eğer nullsa new'lenecek.
            {
                lock (_LockControl) // multi-thread uygulamalarda bu bloğa birden fazla kez girilebilme ihtimali vardır. Bu yüzden lock kullanarak işlemlerin sırayla yapılması sağlanır. Lock bir object ile çalışacağından yukarıda bu nesneyi oluşturmamız gerekiyor.
                {
                    if(_db == null) // 2. bir kontrol ile işi garantiye alırız :D
                    {
                        _db = new DatabaseContext();
                    }
                }
            }
            return _db;
        }
    }
}
