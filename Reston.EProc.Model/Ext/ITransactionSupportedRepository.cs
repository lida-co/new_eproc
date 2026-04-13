using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reston.Eproc.Model.Ext
{
    public interface ITransactionSupportedRepository
    {

        /// <summary>
        /// Tells the underliying <c>DbContext</c> to begin a transaction.
        /// 
        /// If multiple <c>DbContext</c> are used, it is left to the implementation of this interface 
        /// to coordinate transactions accross those <c>DbContext</c>.
        /// 
        /// Implementation of this interface method starts transaction with whatever default isolation
        /// level the underlying <c>DbContext</c> supports. 
        /// </summary>
        /// 
        /// <returns>
        /// the transaction. Can be used to check transactions status, to commit, or to rollback 
        /// the trasaction
        /// </returns>
        DbContextTransaction BeginTransaction();

        /// <summary>
        /// Tells the underliying <c>DbContext</c> to begin a transaction.
        /// 
        /// If multiple <c>DbContext</c> are used, it is left to the implementation of this interface 
        /// to coordinate transactions accross those <c>DbContext</c>es.
        /// 
        /// Implementation of this interface method will always starts transaction with 'Read committed'
        /// isolation level
        /// </summary>
        /// 
        /// <param name="isolationLevel">The isolation level of the transction to start.</param>
        /// 
        /// <returns>
        /// the transaction. Can be used to check transactions status, to commit, or to rollback 
        /// the trasaction
        /// </returns>
        DbContextTransaction BeginTransaction(System.Data.IsolationLevel isolationLevel);

    }
}
