using System.Collections.Generic;

using UnityEngine;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

public abstract class BaseLine<T> : MonoBehaviour where T : IQueueable<T>
{
    public abstract Queue<T> LineQueue { get; set; }

    public abstract Transform[] LinePoints { get; set; }
    public List<T> Content;

    public virtual T PeekFirst()
    {
        if (LineQueue.TryPeek(out T firstMember))
        {
            return firstMember;
        }
        return default;
    }

    public virtual T ReturnFirst()
    {
        if (LineQueue.TryDequeue(out T excistingMember))
        {
            return excistingMember;
        }
        else
        {
            return default;
        }
    }

    public virtual void AddToLine(T item)
    {
        LineQueue.Enqueue(item);
        item.CurrentLine = this;
    }

    [Button]
    public abstract UniTask Initialize(T[] values);

    public int FindMemberIndex(T memberToFind)
    {
        int startIndex = 0;
        foreach (T member in LineQueue)
        {
            if (EqualityComparer<T>.Default.Equals(member, memberToFind))
                return startIndex;

            startIndex++;
        }
        Debug.LogWarning($"{memberToFind} is not a member of {this}");
        return -1;
    }

    public virtual async UniTask OrderQueue()
    {
        List<UniTask> orderingProcesses = new();
        foreach (T member in LineQueue)
        {
            var newMemberStandPoint = LinePoints[FindMemberIndex(member)];
            member.CurrentStandPoint = newMemberStandPoint;
            orderingProcesses.Add(member.Move(newMemberStandPoint.position, 0.5F));
        }
        await UniTask.WhenAll(orderingProcesses);
    }

    [Button]
    public virtual void ClearLine()
    {
        if (LinePoints != null)
        {
            foreach (Transform point in LinePoints)
            {
                if (point) DestroyImmediate(point.gameObject);
            }
        }

        LinePoints = new Transform[0];
        LineQueue?.Clear();
    }
}
