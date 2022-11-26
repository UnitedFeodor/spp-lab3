using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace directory_scanner
{
    public class TaskQueue
    {
        private readonly ConcurrentQueue<Action?> _tasks = new();
        private ushort _taskCount;
        
        private readonly List<Thread> _threads = new();
        private ushort _maxThreadCount;

        private readonly CancellationTokenSource _cancellationToken;

        public TaskQueue(CancellationTokenSource cancellationToken, ushort maxThreadCount)
        {
            _cancellationToken = cancellationToken;
            _taskCount = 0;
            _maxThreadCount = maxThreadCount;
            for (var i = 0; i < _maxThreadCount; i++)
            {
                var thread = new Thread(() =>
                {
              
                    while (!_cancellationToken.Token.IsCancellationRequested)
                    {
                        var task = DequeueTask();
                        try
                        {
                            task.Invoke();
                        }
                        catch (Exception)
                        {
                            
                        }
                    }

                });
                thread.IsBackground = true;

                _threads.Add(thread);
                thread.Start();
            }
        }

        public void EnqueueTask(Action? task)
        {
            lock (_tasks)
            {
                _tasks.Enqueue(task);
                Monitor.Pulse(_tasks);
            }
        }
        private Action? DequeueTask()
        {
            lock (_tasks)
            {
                while (_tasks.Count == 0 && !_cancellationToken.Token.IsCancellationRequested)
                {
                    _taskCount++;
                    Monitor.Wait(_tasks);
                    _taskCount--;
                }
                try
                {
                    Action? retValue;
                    _tasks.TryDequeue(out retValue);
                    return retValue;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public void WaitForEnd()
        {
            while (_taskCount != _maxThreadCount && !_cancellationToken.Token.IsCancellationRequested) 
            {
            
            }
        }

        public void Cancel()
        {
            _cancellationToken.Cancel();
            lock (_tasks)
            {
                Monitor.PulseAll(_tasks); //notify all
            }
            foreach (var thread in _threads)
            {
                thread.Join();
            }
        }


    }

}
