using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechXpress.Data.Contracts
{
    public interface IRepositoryBase<TEntity> where TEntity : class
    {
        TEntity GetById(int id);

        IEnumerable<TEntity> GetAll();

        void Insert(TEntity entity);

        void Update(TEntity entity);

        void Delete(int id);
    }
}
