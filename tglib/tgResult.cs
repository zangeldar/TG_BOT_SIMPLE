using System;
using System.Collections.Generic;
using System.Text;

namespace tglib
{
    //public interface itgResult { }

    public class tgRoot
    {
        public bool ok { get; set; }
        public dynamic result { get; set; }
    }

    public class tgRootGetMe:tgRoot
    {
        //public bool ok { get; set; }
        new public tgUser result { get; set; }
    }

    public class tgRootGetUpdates:tgRoot
    {
        //public bool ok { get; set; }
        new public List<tgUpdate> result { get; set; }
        public List<tgUpdate> getNewUpdatesOnly(long last_known_update_id)
        {
            List<tgUpdate> newUpdates = new List<tgUpdate>();
            foreach (tgUpdate item in result)
                if (item.update_id > last_known_update_id)
                    newUpdates.Add(item);
                else if (Math.Round((double)(last_known_update_id/item.update_id)) != 1)
                    // in case of update_id had been reset, last_known_update_id will be some times more than item.update_id
                    newUpdates.Add(item);
                else { }    // there is an old tgUpdate

            return newUpdates;
        }
    }

    public class tgRootGetChat:tgRoot
    {
        //public bool ok { get; set; }
        new public tgChat result { get; set; }
    }

    public class tgRootCopyMessage:tgRoot
    {
        //public bool ok { get; set; }
        new public copyMsgResult result { get; set; }
    }

    /////////////////////
    ///

    public class copyMsgResult
    {
        public long message_id { get; set; }
    }

}
