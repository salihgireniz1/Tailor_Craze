using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class YarnController : MonoSingleton<YarnController>
{
    [SerializeField] YarnDataContainer _yarnDataContainer;
    public YarnType GetRandomYarnType()
    {
        Array values = Enum.GetValues(typeof(YarnType));
        return (YarnType)values.GetValue(UnityEngine.Random.Range(0, values.Length));
    }
    public YarnData GetYarnData(YarnType yarnType)
    {
        return _yarnDataContainer.yarnGroups.First(yarnData => yarnData.Type == yarnType);
    }
    public Yarn GetYarn(bool isHidden)
    {
        switch (isHidden)
        {
            case true:
                return Instantiate(_yarnDataContainer.HiddenYarn);
            default:
                return Instantiate(_yarnDataContainer.BaseYarn);
        }

    }
    public Yarn GetYarn(YarnType yarnType)
    {
        try
        {
            var yarnData = GetYarnData(yarnType);
            Yarn newYarn = Instantiate(_yarnDataContainer.BaseYarn);
            // newYarn.InitializeYarn(yarnData);
            return newYarn;
        }
        catch (System.Exception)
        {
            Debug.LogError($"There is no {yarnType} yarn.");
            return null;
        }
    }
    public UniTask Rolling(Yarn yarn, RollType rollType, BaseSpool yarnSpool, float duration)
    {
        float rollDir = rollType == RollType.Roll ? -1f : 1f;
        yarnSpool.rollDir = rollDir;


        return DOTween.To(
                () => yarn.Tube.clipTo,
                clip => yarn.Tube.clipTo = clip,
                rollType == RollType.Roll ? 1f : 0f,
                duration
            )
            .SetEase(Ease.Linear)
            .ToUniTask();
    }
}
public enum RollType { Roll, UnRoll }