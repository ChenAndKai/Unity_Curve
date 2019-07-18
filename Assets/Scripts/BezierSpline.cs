using System;
using UnityEngine;

public class BezierSpline : MonoBehaviour
{


    [SerializeField]
    private Vector3[] points;                   //序列化点

    [SerializeField]
    private BezierControlPointMode[] modes;     //序列化三个模式

    [SerializeField]
    private bool loop;

    /// <summary>
    /// 曲线第一个和最后一个点共享同样的位置，把样条曲线转变成线圈
    /// </summary>
    public bool Loop
    {
        get
        {
            //返回当前loop开关状态
            return loop;
        }
        set
        {
            //loop这个开关选中与否
            loop = value;
            Debug.Log(value);
            if (value == true)
            {
                modes[modes.Length - 1] = modes[0];
                SetControlPoint(0, points[0]);
            }
        }
    }

    #region 控制点私有化之后，其他脚本不能直接访问point，只能通过以下三个方法获取相应数据以及做相应的改变
    /// <summary>
    /// 获取控制点的数量
    /// </summary>
    public int ControlPointCout
    {
        get
        {
            return points.Length;
        }
    }

    /// <summary>
    /// 获取当前控制点
    /// </summary>
    /// <param name="index">控制点索引</param>
    /// <returns></returns>
    public Vector3 GetControlPoint(int index)
    {
        return points[index];
    }

    /// <summary>
    /// 设置控制点
    /// </summary>
    /// <param name="index">控制点索引</param>
    /// <param name="point">世界坐标系下的控制点</param>
    public void SetControlPoint(int index, Vector3 point)
    {
        //强制约束，不论移动一个点或者改变点的模式，约束会被执行。但当移动中间点时，以前的点总是保持固定，下一个点总是被执行
        //如果改变的点是中间点，即两条曲线的交点
        if (index % 3 == 0)
        {
            //不明白point 为什么不等于 points[index]?;
            Vector3 delta = point - points[index];
            //循环成线圈
            if (loop) {
                //第一个点
                if (index == 0)
                {
                    points[1] += delta;
                    points[points.Length - 2] += delta;
                    points[points.Length - 1] = point;
                }
                //最后一个点
                else if (index == points.Length - 1)
                {
                    points[0] = point;
                    points[1] += delta;
                    points[index - 1] += delta;
                }
                else
                {
                    points[index - 1] += delta;
                    points[index + 1] += delta;
                }
            }
            //不循环
            else
            {
                //如果选中了一个不是起点的点
                if (index > 0)
                {
                    points[index - 1] += delta;
                }
                //如果选中的不是整一段曲线的末点
                if (index + 1 < points.Length)
                {

                    points[index + 1] += delta;
                }
            }
           
        }
        points[index] = point;

        EnforceMode(index);
    }
    #endregion

    /// <summary>
    /// 当点被移动或模式被改变。约束会被执行，(所以真正目的是什么？）
    /// </summary>
    /// <param name="index"></param>
    private void EnforceMode(int index)
    {
        int modeIndex = (index + 1) / 3;
        BezierControlPointMode mode = modes[modeIndex];
        //当mode的模式是Free或selectIndex=-1(未选中任何点)或是模式数组的最后一个，直接返回
        if (mode == BezierControlPointMode.Free || modeIndex == 0||modeIndex==modes.Length-1)
        {
            return;
        }
        //index         0 1 2 3 4 5 6  
        //modeIndex     0 0 1 1 1 2 2 
        //middleIndex   0 0 3 3 3 6 6 
        int middleIndex = modeIndex * 3;            
        int fixedIndex, enforcedIndex;
        if(index<=middleIndex)
        {
            fixedIndex = middleIndex - 1;
            enforcedIndex = middleIndex + 1;
        }
        else
        {
            fixedIndex = middleIndex + 1;
            enforcedIndex = middleIndex - 1;
        }
        //拿4举例，index=4,middleIndex=3,fixedIndex=4,enforcedIndex=2;(2,3,4的中间点是3)
        //先求第四个点到第三个点的向量（middle-points[fixedIndex]）
        //为了保证两条曲线的连接点（index为3的点）处斜率一致，第三个点到第二个点的向量应该要是前面求得的向量
        //因此第二个点是第三个点middle加上前面求得的向量
        Vector3 middle = points[middleIndex];
        Vector3 enforcedTangent = middle - points[fixedIndex];

        //调整模式下,确定新的正切线与旧的有相同的长度
        if (mode == BezierControlPointMode.Aligned)
        {
            enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
        }
        points[enforcedIndex] = middle + enforcedTangent;
    }


    #region 每条曲线的第四个点是下一条曲线的第一个点，此时的索引是3的倍数（从0计数）,这里的目的是为了这个点的前一个和后一个点的模式一致
    //因此我们需要转换点的索引成模式索引因为在现实中，点共享模式。
    //例如，点的索引序列0,1,2,3,4,5,6,7,8,9与模式索引序列0,0,1,1,1,2,2,2,3,3对应。增加一条新曲线就会增加三个点。
    /// <summary>
    /// 获取点的模式
    /// </summary>
    /// <param name="index">所选择点的索引</param>
    /// <returns></returns>
    public BezierControlPointMode GetControlPointMode(int index)
    {
        return modes[(index + 1) / 3];
    }

    /// <summary>
    /// 将当前所选择点的模式改为前面所选择的模式
    /// </summary>
    /// <param name="index">所选择点的索引</param>
    /// <param name="mode">前面所选择的模式</param>
    public void SetControlPointMode(int index,BezierControlPointMode mode)
    {
        int modeIndex = (index + 1) / 3;
        modes[modeIndex] = mode;

        //为形成线圈，正确执行循环，确保第一个和最后一个点的模式在循环中保持相同
        if (loop)
        {
            if(modeIndex == 0)
            {
                modes[modes.Length - 1] = mode;
            }
        }
        else if(modeIndex == modes.Length-1)
        {
            modes[0] = mode;
        }
        EnforceMode(index);
    }
    #endregion


    /// <summary>
    /// 获取曲线上的点，并转换成世界坐标系下的点
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public Vector3 GetPoint(float t)
    {
        //t在[0,1]，t*CurveCount  取整作为当前曲线的索引，小数部分是当前曲线上插值的系数，当t=1，直接认为是最后一段曲线
        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3; //i*3代表第（i+1）段曲线的起始点索引
        }
        return transform.TransformPoint(Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t));
    }

    /// <summary>
    /// 获取曲线上某一点的速度向量
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public Vector3 GetVelocity(float t)
    {
        //t在[0,1]，t*CurveCount  取整作为当前曲线的索引，小数部分是当前曲线上插值的系数，当t=1，直接认为是最后一段曲线
        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3; //i*3代表第（i+1）段曲线的起始点索引
        }
        return transform.TransformPoint(Bezier.GetFirstDerivative(points[i], points[i+1], points[i+2], points[i+3], t)) -
            transform.position;
    }

    /// <summary>
    /// 速度标准化
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public Vector3 GetDirection(float t)
    {
        return GetVelocity(t).normalized;
    }

    /// <summary>
    /// 添加新一条的样条曲线，但是考虑到曲线的连贯性，前一条曲线的终点是新曲线的起点，所以再添加三个点
    /// </summary>
    public void AddCurve()
    {
        //曲线上的最后一个点
        Vector3 point = points[points.Length - 1];
        //在原数组内容不变情况下，在后面新增加3个单位的空间，此时length：7
        Array.Resize(ref points, points.Length + 3);
        //最后一个点的x坐标加1
        point.x += 1f;
        //points[4] = [point.x, point.y, point.z];
        points[points.Length - 3] = point;
        point.x += 1f;
        points[points.Length - 2] = point;
        point.x += 1f;
        points[points.Length - 1] = point;
        
        //每增加一条曲线就要增加三个点，其中第一个点的模式和前一个点属于同一个模式，后两个点属于同一个模式，故长度要加1
        Array.Resize(ref modes, modes.Length + 1);
        //新加的模式初始化成前一个模式
        modes[modes.Length - 1] = modes[modes.Length - 2];

        //添加一条曲线，传入两条曲线的交点索引
        EnforceMode(points.Length - 4);

        //在开启loop情况下，若再增加一条曲线，依旧保持正确的圆形
        if(loop)
        {
            points[points.Length - 1] = points[0];
            modes[modes.Length - 1] = modes[0];
            EnforceMode(0);
        }

    }

    /// <summary>
    /// 获取曲线的段数
    /// </summary>
    public int CurveCount
    {
        get
        {
            return (points.Length - 1) / 3;
        }
    }

    /// <summary>
    /// 变量初始化（点，模式）
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

        //第一条线段有两个模式索引（第一、二个点的模式：modes[0],第三、四个点的模式:modes[1]）都初始化成Free
        modes = new BezierControlPointMode[]
        {
            BezierControlPointMode.Free,
            BezierControlPointMode.Free
        };
    }

    

}
