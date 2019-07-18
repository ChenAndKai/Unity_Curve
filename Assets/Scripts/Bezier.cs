using UnityEngine;

/// <summary>
/// Bezier类，为任意的连续点做运算
/// </summary>
public static class Bezier
{
    /// <summary>
    /// 二次曲线
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="t">[0,1]范围内的值</param>
    /// <returns></returns>
    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        //Vector3.Lerp(p0,p1,t)
        //p0 + (p1 - p0) * t
        //线性曲线:B(t) = (1-t)p0 + tp1
        //return Vector3.Lerp(p0, p2, t);


        //将t限制在[0,1]范围
        //B(t) = (1-t)^2*p0 + 2(1-t)*t*p1 +t^2 * p2 
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return oneMinusT * oneMinusT * p0 +
            2f * oneMinusT * t * p1 +
            t * t * p2;
    }
    /// <summary>
    /// 对二次贝塞尔曲线一阶求导
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        //二次贝塞尔曲线的一阶导：B'(t) = 2(1-t)(p1-p0)+2t(p2-p1)
        //返回速度向量，会受到曲线对象位置影响
        return 2f * (1f - t) * (p1 - p0) +
               2f * t * (p2 - p1);
    }



    /// <summary>
    /// 三次曲线
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        //B(t)=(1-t)^3 * p0 +3(1-t)^2 * t*p1 +3(1-t)*t^2 *p2+3(1-t)*t^2 * p2 + t^3 * p3
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;

        return oneMinusT * oneMinusT * oneMinusT * p0 +
                3f * oneMinusT * oneMinusT * t * p1 +
                3f * oneMinusT * t * t * p2 +
                t * t * t * p3;
    }

    /// <summary>
    /// 三次曲线的一阶求导
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <param name="t"></param>
    public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {

        //B'(t) = 3(1-t)^2(p1-p0)+6(1-t)t(p2-p1)+3t^2(p3-p2)
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return 3f * oneMinusT * oneMinusT * (p1 - p0) +
               6f * oneMinusT * t * (p2 - p1) +
               3f * t * t * (p3 - p2);
    }
}
