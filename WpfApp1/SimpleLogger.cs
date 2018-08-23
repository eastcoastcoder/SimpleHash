using System;
using System.Collections.Generic;
using System.Threading;

namespace WpfApp1
{
    public class SimpleLogger
    {
        private Mutex _mutex = new Mutex(false);
        private UInt32 _entryCount = 0;
        public class Entry
        {
            public enum EntryType
            {
                Info,
                Warning,
                Error,
                Fault
            }

            private EntryType _entryType;
            private DateTime _timestamp;
            private String _msg;
            private UInt32 _index;

            private Entry()
            {
            }

            public Entry(EntryType entryType, String msg, UInt32 index)
            {
                _msg = msg;
                _timestamp = DateTime.Now;
                _entryType = entryType;
                _index = index;
            }

            public String msg { get { return _msg; } }
            public DateTime timestamp { get { return _timestamp; } }
            public EntryType entryType { get { return _entryType; } }
            public UInt32 index { get { return _index; } }

            public override string ToString()
            {
                return String.Format("[{0}][{1}][{2}][{3}]", _timestamp,_index, _entryType, _msg);
            }
        }

        private List<Entry> _entries = new List<Entry>();

        public void Reset()
        {
            try
            {
                _mutex.WaitOne();
                _entries = new List<Entry>();
            }
            finally
            {
                _mutex.ReleaseMutex();                
            }
        }

        public Int32 Count
        {
            get
            {
                _mutex.WaitOne();
                Int32 result = _entries.Count; _mutex.ReleaseMutex(); return result;
            }
        }

        /// <summary>
        /// Gets the first entry in log and removes it from the log.
        /// Returns null if the log is empty.
        /// </summary>
        /// <returns></returns>
        public Entry GetFirst()
        {
            Entry result = null;
            try
            {
                _mutex.WaitOne();
                if (_entries.Count > 0)
                {
                    result = _entries[0];
                    _entries.RemoveAt(0);
                }

            }
            finally
            {
                _mutex.ReleaseMutex();                
            }
            return result;
        }

        /// <summary>
        /// Retrives all the entries from the log.  The log will be 
        /// empty after the operation has been executed.
        /// </summary>
        /// <returns></returns>
        public List<Entry> GetAll()
        {
            List<Entry> result = null;
            try
            {
                _mutex.WaitOne();
                result = _entries;
                _entries = new List<Entry>();
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
            return result;
        }

        public void AddInfo(String msg)
        {
            try
            {
                _mutex.WaitOne();
                _entries.Add(new Entry(Entry.EntryType.Info, msg,_entryCount++));
            }
            finally
            {
                _mutex.ReleaseMutex();                
            }
        }

        public void AddWarning(String msg)
        {
            try
            {
                _mutex.WaitOne();
                _entries.Add(new Entry(Entry.EntryType.Warning, msg, _entryCount++));
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public void AddError(String msg)
        {
            try
            {
                _mutex.WaitOne();
                _entries.Add(new Entry(Entry.EntryType.Error, msg, _entryCount++));
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public void AddFault(String msg)
        {
            try
            {
                _mutex.WaitOne();
                _entries.Add(new Entry(Entry.EntryType.Fault, msg, _entryCount++));
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public void AddFault(Exception ex)
        {

            try
            {
                _mutex.WaitOne();
                String msg = ex.Message;
                if (ex.InnerException != null)
                    msg += " (+INNER): " + ex.InnerException.Message;
                _entries.Add(new Entry(Entry.EntryType.Fault, msg, _entryCount++));
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }
    }
}
