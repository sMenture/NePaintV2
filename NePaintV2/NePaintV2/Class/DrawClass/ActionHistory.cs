using System.Collections.Generic;
using System.Linq;
namespace NePaintV2.Class.DrawClass
{
    public class ActionHistory
    {
        private Stack<byte[]> actions = new Stack<byte[]>();
        public void AddAction(byte[] action)
        {
            actions.Push(action);
        }
        public byte[] Undo()
        {
            if (actions.Count > 0)
            {
                return actions.Pop();
            }
            return null;
        }

        public List<byte[]> GetAllActions()
        {
            return actions.ToList();
        }

        public void SetActions(List<byte[]> newActions)
        {
            actions.Clear();
            foreach (var action in newActions)
            {
                actions.Push(action);
            }
        }
    }
}