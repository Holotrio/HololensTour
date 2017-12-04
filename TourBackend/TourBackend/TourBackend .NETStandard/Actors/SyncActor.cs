﻿using System;
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
    public class SyncActor : IActor
    {

        private Stopwatch stopwatch;

        protected string id { get; }
        public SyncObject syncobject;


        /*The SyncActor has an internal stopwatch which is used to distinguish between frames
         and offers are comparable metric for performance*/

        public SyncActor(string _id, SyncObject _syncobject)
        {
            id = _id;
            syncobject = _syncobject;
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        public Task ReceiveAsync(IContext context)
        {
            var msg = context.Message;
            switch (msg)
            {
                /*This message is sent by the ControlActor and signifies that the SyncActor
                 should make the current data available to the user*/

                case WriteCurrentTourState w:
                    lock (syncobject.thisLock)
                    {
                        syncobject.SetTimeStamp(stopwatch.ElapsedMilliseconds);
                        syncobject.dict = new Dictionary<int, CodeObject>(w.dict);
                    }
                    context.Sender.Tell(new RespondWriteCurrentTourState(w.id));
                    Console.WriteLine("SyncActor says: Current State was written.");
                    break;
            }
            return Actor.Done;
        }

    }
}
