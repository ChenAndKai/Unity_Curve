
using UnityEngine;


/// <summary>
/// 贝塞尔曲线
/// </summary>
public class BezierCurve : MonoBehaviour
{
    public Vector3[] points;
    /// <summary>
    /// 当绑定该脚本的对象创建或重置时，Unity editor 会自动调用Reset方法初始化四个点的值
    /// </summary>
    public void Reset()
    {
        points = new Vector3[]
        {
            new Vector3(1f,0f,0f),
            new Vector3(2f,0f,0f),
            new Vector3(3f,0f,0f),
            new Vector3(4f,0f,0f)
        };
    }

    /// <summary>
    /// 对点进行线性差值画曲线
    /// </summary>
    /// <param name="p0">起始点</param>
    /// <param name="p1">中间点</param>
    /// <param name="p2">终点</param>
    /// <param name="t"></param>
    /// <returns></returns>
    //public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    //{
    //    //from + (to - from) * t
    //    return Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t);
    //}

    /// <summary>
    /// （二次曲线）传入t值，线性插值后，将结果（曲线上的点）转换成世界坐标系
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    //public Vector3 GetPoint(float t)
    //{

    //    return transform.TransformPoint(Bezier.GetPoint(points[0], points[1], points[2] ,t));
    //}


    /// <summary>
    /// （二次曲线）获取曲线某一点的切线（沿着曲线运动的速度），返回速度向量
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    //public Vector3 GetVelocity(float t)
    //{

    //    return transform.TransformPoint(Bezier.GetFirstDerivative(points[0], points[1], points[2], t))-transform.position;
    //}

    /// <summary>
    /// （三次曲线）传入t值，线性插值后，将结果（曲线上的点）转换成世界坐标系
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public Vector3 GetPoint(float t)
    {
        return transform.TransformPoint(Bezier.GetPoint(points[0], points[1], points[2], points[3], t));
    }


    /// <summary>
    /// （三次曲线）获取曲线某一点的切线（沿着曲线运动的速度），返回速度向量
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public Vector3 GetVelocity(float t)
    {
        //TransformPoint()：会考虑缩放的转换
        //transform.position:曲线对象的位置
        return transform.TransformPoint(Bezier.GetFirstDerivative(points[0], points[1], points[2], points[3], t))
                - transform.position;
    }

    /// <summary>
    /// 对速度进行标准化
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public Vector3 GetDirection (float t)
    {
        return GetVelocity(t).normalized;
    }
}
