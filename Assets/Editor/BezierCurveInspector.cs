using UnityEditor;
using UnityEngine;

//自定义BezierCurve脚本，必须继承自Editor
[CustomEditor(typeof(BezierCurve))]
public class NewBehaviourScript : Editor
{
    private BezierCurve curve;
    private Transform handleTransform;
    private Quaternion handleRotation;

    private const int lineSteps = 10;
    #region 我的三次贝塞尔曲线绘制方法
    //private void OnSceneGUI()
    //{

    //curve = target as BezierCurve;
    //handleTransform = curve.transform;
    //handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;


    //Vector3 p0 = ShowPoint(0);
    //Vector3 p1 = ShowPoint(1);
    //Vector3 p2 = ShowPoint(2);
    //Vector3 p3 = ShowPoint(3);

    //Handles.color = Color.gray;
    //Handles.DrawLine(p0, p1);
    //Handles.DrawLine(p2, p3);


    ////Vector3 lineStart = curve.Getpoint(0f);
    ////Handles.color = Color.white;
    ////for (int i = 1; i <= lineSteps; i++)
    ////{
    ////    Vector3 lineEnd = curve.Getpoint(i / (float)lineSteps);
    ////    Handles.DrawLine(lineStart, lineEnd);
    ////    lineStart = lineEnd;
    ////}

    //Vector3 lineStart = curve.GetPoint(0f);
    //Handles.color = Color.green;
    //Handles.DrawLine(lineStart, lineStart + curve.GetDirection(0f));
    ////进行多次线性插值，用循环，将曲线上相邻两点依次画直线，最终形成曲线
    //for (int i = 1; i <= lineSteps; i++)
    //{
    //    Vector3 lineEnd = curve.GetPoint(i / (float)lineSteps);
    //    Handles.color = Color.white;
    //    Handles.DrawLine(lineStart, lineEnd);
    //    Handles.color = Color.green;
    //    Handles.DrawLine(lineEnd, lineEnd + curve.GetDirection(i / (float)(lineSteps)));
    //    lineStart = lineEnd;
    //}

    //}
    #endregion


    #region 系统封装的三次贝塞尔曲线绘制方法
    private const float directionScale = 0.5f;
    private void OnSceneGUI()
    {
        curve = target as BezierCurve;
        handleTransform = curve.transform;
        //Quaternion.identity:无旋转
        handleRotation = Tools.pivotRotation == PivotRotation.Local ?
            handleTransform.rotation : Quaternion.identity;

        Vector3 p0 = ShowPoint(0);
        Vector3 p1 = ShowPoint(1);
        Vector3 p2 = ShowPoint(2);
        Vector3 p3 = ShowPoint(3);

        Handles.color = Color.gray;
        Handles.DrawLine(p0, p1);
        Handles.DrawLine(p2, p3);

        ShowDirections();
        Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);

    }
    private void ShowDirections()
    {
        Handles.color = Color.green;
        Vector3 point = curve.GetPoint(0f);
        Handles.DrawLine(point, point + curve.GetDirection(0f) * directionScale);
        for(int i=1;i<=lineSteps;i++)
        {
            point = curve.GetPoint(i / (float)lineSteps);
            Handles.DrawLine(point, point + curve.GetDirection(i / (float)lineSteps) * directionScale);
        }
    }

    #endregion

    /// <summary>
    /// 显示点
    /// </summary>
    /// <param name="index">(曲线上三个点的索引)</param>
    /// <returns></returns>
    private Vector3 ShowPoint(int index)
    {
        //世界坐标系下的点
        Vector3 point = handleTransform.TransformPoint(curve.points[index]);
        EditorGUI.BeginChangeCheck();
        point = Handles.DoPositionHandle(point, handleRotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(curve, "Move Point");
            EditorUtility.SetDirty(curve);
            //将位置改动后的世界坐标系中的点坐标转换成脚本绑定对象定义的点坐标
            curve.points[index] = handleTransform.InverseTransformPoint(point);
        }
        return point;
    }
}
