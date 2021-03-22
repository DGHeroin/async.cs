using System;
using System.Collections.Generic;

public class Async {
        public delegate void NextCallback(object err, params object[] args);
        public delegate void Callback(object err, NextCallback next, params object[] args);
        public static Context Run(params Callback[] actions) {
            Context ctx = new Context(actions);
            return ctx;
        }

        public class Context {
            private NextCallback onComplete;
            private List<Callback> callbacks = new List<Callback>();
            private bool isStart = false;
            private object lastErr;
            public Context(Callback[] actions) {
                callbacks.AddRange(actions);
            }
            public void OnComplete(NextCallback cb) {
                onComplete = cb;
                isStart = true;
                Next();
            }
            public void Start() {
                if (isStart) return;
                isStart = true;
                Next();
            }
            private void Next(params object[] args) {
                if(args==null) {
                    args = new object[0];
                }
                if (callbacks.Count == 0) {
                    if (onComplete != null) {
                        onComplete(lastErr, args);
                    }
                    return;
                }
                var cb = callbacks[0];
                callbacks.RemoveAt(0);
                cb(lastErr, (err, args2) => {
                    if (args2 == null) {
                        args2 = new object[0];
                    }
                    if (err != null) {
                        lastErr = err;
                        callbacks.Clear();
                        Next(args2);
                        return;
                    }
                    Next(args2);
                }, args);
            }
        }
    }
