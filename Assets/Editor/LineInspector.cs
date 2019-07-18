using UnityEngine;
using UnityEditor;

//自定义Line脚本，必须继承自Editor
[CustomEditor(typeof(Line))]
public class LineInspector : Editor
{

   
    /// <summary>
    /// 在scene视图中绘制元素
    /// </summary>
    /// 
    private void OnSceneGUI()
    {
        //target:Editor内变量，代表鼠标选中的对象（Hieraychy或者scene中）
        Line line = target as Line;
        //不能对线进行操作,因为Handlesa是在世界坐标系下操作，而两个点坐标是相对于Line对象的局部坐标系
        //将这些点转变到世界坐标系
        Transform handleTransform = line.transform;
        //获取点的坐标轴（xyz轴），手柄
        Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;
        Vector3 p0 = handleTransform.TransformPoint(line.p0);
        Vector3 p1 = handleTransform.TransformPoint(line.p1);

        Handles.color = Color.white;
        Handles.DrawLine(p0, p1);
        //位置控制手柄
        Handles.DoPositionHandle(p0, handleRotation);
        Handles.DoPositionHandle(p1, handleRotation);

        //移动两点坐标轴，Line脚本中的值也会发生改变。
        //InverseTransformPoint():把坐标转换为Line脚本绑定对象的局部坐标系。
        //调用EditorGUI.BeginChangeCheck()方法之后，EditorGUI.EndChangeCheck()方法（返回True/False）告诉我们改变是否发生
        EditorGUI.BeginChangeCheck();
        p0 = Handles.DoPositionHandle(p0, handleRotation);
        if (EditorGUI.EndChangeCheck())
        {
            //撤销点的拖动
            Undo.RecordObject(line, "Move point");
            //保存改变记录
            EditorUtility.SetDirty(line);
            line.p0 = handleTransform.InverseTransformPoint(p0);
        }
        EditorGUI.BeginChangeCheck();
        p1 = Handles.DoPositionHandle(p1, handleRotation);
        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(line, "Move point");
            EditorUtility.SetDirty(line);
            line.p1 = handleTransform.InverseTransformPoint(p1);
        }
    }

}

