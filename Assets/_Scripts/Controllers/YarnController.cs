using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class YarnController : MonoSingleton<YarnController>
{
    [SerializeField] YarnDataContainer _yarnDataContainer;
    [SerializeField] public float _rollDuration;
    [SerializeField] Transform _spoolBody;
    public YarnData GetYarnData(YarnType yarnType)
    {
        return _yarnDataContainer.yarnGroups.First(yarnData => yarnData.Type == yarnType);
    }
    public Yarn GetYarn(YarnType yarnType)
    {
        try
        {
            var yarnData = GetYarnData(yarnType);
            // var yarn = yarnData.Yarns.First(yarn => yarn.RollLength == length);
            Yarn newYarn = Instantiate(_yarnDataContainer.BaseYarn);
            newYarn.InitializeYarn(yarnData);
            return newYarn;
        }
        catch (System.Exception)
        {
            Debug.LogError($"There is no {yarnType} yarn.");
            return null;
        }
    }
    public UniTask Rolling(Yarn yarn, RollType rollType, BaseSpool yarnSpool)
    {
        float rollDir = rollType == RollType.Roll ? -1f : 1f;
        yarnSpool.rollDir = rollDir;
        // Calculate the total rotation amount needed (1080 degrees = 360 * 3)
        float totalRotation = 1080f;
        // Calculate the rotation per second
        float rotationSpeed = totalRotation / _rollDuration;

        return DOTween.To(
                () => yarn.Tube.clipTo,
                clip => yarn.Tube.clipTo = clip,
                rollType == RollType.Roll ? 1f : 0f,
                _rollDuration
            )
            .SetEase(Ease.Linear)
            // .OnUpdate(
            //     () =>
            //     {
            //         // Rotate the spool by the calculated amount per frame
            //         yarnSpool.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime * rollDir);
            //         yarnSpool.transform.Rotate(0f, rotationSpeed * rollDir * Time.deltaTime, 0f);
            //         foreach (var item in yarnSpool._contents)
            //         {
            //             item.Spline.RebuildImmediate();
            //         }
            //     }
            // )
            .ToUniTask();
    }
    public float RollDuration => _rollDuration;
}
public enum RollType { Roll, UnRoll }