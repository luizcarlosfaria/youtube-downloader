using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;

namespace DevWeek.Services
{
    /// <summary>
    /// Lock resources with Redis Distributed Lock pattern
    /// </summary>
    public class DistributedLockService
    {
        private readonly ConnectionMultiplexer redisClient;


        /// <summary>
        /// Initialize a DistributedLockService with a ConnectionMultiplexer
        /// </summary>
        /// <param name="redisClient"></param>
        public DistributedLockService(ConnectionMultiplexer redisClient)
        {
            this.redisClient = redisClient;
        }

        /// <summary>
        /// Get and Set a TimeSpan that define a retry waiting for get a lock
        /// </summary>
        public TimeSpan WaitingLockCycle { get; set; }

        /// <summary>
        /// Aquire a lock and returns a DistributedLock instance.
        /// </summary>
        /// <remarks>
        /// DistributedLock implements IDisposable and can be used in using statement
        /// using (distributedLockService.Acquire(0, "lockKey", TimeSpan.FromSeconds(50)))
        /// {
        ///     //your code here!
        /// }
        /// </remarks>
        /// <param name="databaseIndex">Redis database Index</param>
        /// <param name="key">Redis lock key</param>
        /// <param name="lockTimeOut">Time locking the resource</param>
        /// <returns>Returns a DistributedLock to manage lock</returns>
        public DistributedLock Acquire(int databaseIndex, string key, TimeSpan lockTimeOut)
        {
            return new DistributedLock(redisClient, databaseIndex, key, lockTimeOut, this.WaitingLockCycle, true);
        }


        /// <summary>
        /// Lock some resource, execute a action and release lock after execution
        /// </summary>
        /// <param name="databaseIndex">Redis database Index</param>
        /// <param name="key">Redis lock key</param>
        /// <param name="lockTimeOut">Time locking the resource</param>
        /// <param name="runnable">Action to run when lock is acquired</param>
        public void WithLock(int databaseIndex, string key, TimeSpan lockTimeOut, Action runnable)
        {
            using (this.Acquire(databaseIndex, key, lockTimeOut))
            {
                runnable();
            }
        }


        /// <summary>
        /// Define a distributed lock
        /// </summary>
        public class DistributedLock : IDisposable
        {
            private ConnectionMultiplexer redisClient;
            private TimeSpan lockTimeOut;
            private TimeSpan waitingLockCycle;
            private IDatabase db;

            /// <summary>
            /// Redis database Index
            /// </summary>
            public int DatabaseIndex { get; }

            /// <summary>
            /// Redis lock key
            /// </summary>
            public string Key { get; }

            /// <summary>
            /// Unique ID used for lock owner control
            /// </summary>
            public string LockId { get; }


            /// <summary>
            /// Create a lock
            /// </summary>
            /// <param name="redisClient">StackExchange.Redis.ConnectionMultiplexer used for connect to Redis</param>
            /// <param name="databaseIndex">Redis database Index</param>
            /// <param name="key">Redis lock key</param>
            /// <param name="lockTimeOut">Lock timeout</param>
            /// <param name="waitingLockCycle">Time to waiting for retry acquire lock</param>
            /// <param name="acquireNow">Try acquire lock on lock creation</param>
            public DistributedLock(ConnectionMultiplexer redisClient, int databaseIndex, string key, TimeSpan lockTimeOut, TimeSpan waitingLockCycle, bool acquireNow)
            {
                this.redisClient = redisClient;
                this.DatabaseIndex = databaseIndex;
                this.Key = key;
                this.lockTimeOut = lockTimeOut;
                this.waitingLockCycle = waitingLockCycle;
                this.db = this.redisClient.GetDatabase(this.DatabaseIndex);
                this.LockId = Guid.NewGuid().ToString("N");

                if (acquireNow)
                    this.Acquire();
            }

            /// <summary>
            /// Try acquire a lock
            /// </summary>
            public void Acquire()
            {
                while (!this.db.LockTake(this.Key, this.LockId, lockTimeOut))
                {
                    System.Threading.Thread.Sleep(this.waitingLockCycle);
                }
            }

            /// <summary>
            /// Try release a lock
            /// </summary>
            public void Release()
            {
                this.db.LockRelease(this.Key, this.LockId);
            }

            /// <summary>
            /// IDisposable required method, used on finish using statement
            /// </summary>
            public void Dispose()
            {
                this.Release();
            }
        }

    }
}
