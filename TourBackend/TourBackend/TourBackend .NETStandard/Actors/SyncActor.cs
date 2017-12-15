using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proto;
using System.Reflection;
using System.Threading;
using System.Diagnostics;

namespace TourBackend
{
    /// <summary>
    /// Essential actor in the system. Manages the interaction with output data on the SyncObject.
    /// </summary>
    public class SyncActor : IActor
    {

        private Stopwatch stopwatch;

        protected string id { get; }
        public SyncObject syncobject;


        /*The SyncActor has an internal stopwatch which is used to distinguish between frames
         and offers are comparable metric for performance*/


        /// <summary>
        /// Started by ControlActor. [Internal Use]
        /// </summary>
        /// <param name="_id">Name to be given to the SyncActor</param>
        /// <param name="_syncobject">User provided SyncObject</param>
        public SyncActor(string _id, SyncObject _syncobject)
        {
            id = _id;
            syncobject = _syncobject;
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        /// <summary>
        /// Behaviour of SyncActor. [Internal Use]
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task ReceiveAsync(IContext context)
        {
            var msg = context.Message;
            switch (msg)
            {
                /*This message is sent by the ControlActor and signifies that the SyncActor
                 should make the current data available to the user*/

                case WriteCurrentTourState w:
                    Debug.WriteLine($"Tesla: {String.Join(",", w.IDToCodeObject.Keys)}");
                    lock (syncobject.thisLock)
                    {
                        syncobject.SetTimeStamp(stopwatch.ElapsedMilliseconds);
                        try
                        {
                            syncobject.dict = CopySyncDict.Copy(w.IDToCodeObject);
                        }
                        catch (Exception e) {
                            Console.WriteLine(e.StackTrace);
                        }
                    }
                    context.Sender.Tell(new RespondWriteCurrentTourState(w.id));
                    syncobject.UpdateSyncObject();
                    Console.WriteLine("SyncActor says: Current State was written.");
                    break;
            }
            return Actor.Done;
        }

    }
}
