using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using GoatAutoCAD.baseutil;
using GoatAutoCAD.constant;
using GoatAutoCAD.db;

namespace GoatAutoCAD.interaction  {

    public static class InteractionUtil {

        public static double getAngle(string message) {
            PromptDoubleResult res = GoatDB.ed.GetAngle(message);
            if (res.Status == PromptStatus.OK) return res.Value;
            return double.NaN;
        }

        /// <summary> ed.GetDouble(new PromptDoubleOptions(message) { AllowNone = true })
        /// 提示用户输入整数
        /// </summary>
        /// <param name="msg">用户输入命令后的提示信息</param>
        /// <returns>返回Point3d</returns>
        /// <summary>
        public static int getInteger(string msg){
            PromptIntegerOptions options = new PromptIntegerOptions(msg){ AllowNone = false };
            PromptIntegerResult pt = GoatDB.ed.GetInteger(options);
            if (pt.Status == PromptStatus.OK) return pt.Value;
            return 0;
        }

        /// <summary>
        ///  获取用户输入的距离
        /// </summary>
        /// <param name="message">用户输入命令后的提示信息</param>
        /// <returns> 用户选择的两个点之间的距离</returns>
        public static double getDistance(string message) {
            var res = GoatDB.ed.GetDistance(message);
            if (res.Status == PromptStatus.OK) return res.Value;
            return double.NaN;
        }

        /// <summary>
        /// 提示用户拾取点
        /// </summary>
        /// <param name="msg">用户输入命令后的提示信息</param>
        /// <param name="allowNone"> 是否允许用户输入空值  true 直接输入回车后自动选择0点  false 直接输入回车后提示无效点</param>
        /// <returns>返回 用户选择点的坐标点</returns>
        public static Point3d getPoint(string msg,bool allowNone = false){
            PromptPointOptions options = new PromptPointOptions(msg);
            options.AllowNone = allowNone;
            PromptPointResult pt = GoatDB.ed.GetPoint(options);
            if (pt.Status != PromptStatus.OK) return new Point3d();
            return pt.Value;
        }

        /// <summary>
        /// 提示用户在 Command 提示光标处输入一个字符串
        /// </summary>
        /// <param name="message">用户输入命令后的提示信息</param>
        /// <param name="defaultValue"> 如果用户不输入 则取默认值</param>
        /// <param name="allowSpaces">AllowSpaces 属性控制是否可以输入空格，如果设置为 false，按空格键就终止输入 默认为 false sos 该参数测试无效</param>
        /// <returns>The string.</returns>
        public static string getString(string message, string defaultValu = null,bool allowSpaces = false) {
            PromptStringOptions opt = new PromptStringOptions(message);
            opt.DefaultValue = defaultValu;
            opt.AllowSpaces = allowSpaces;
            PromptResult res = GoatDB.ed.GetString(new PromptStringOptions(message) { DefaultValue = defaultValu });
            if (res.Status == PromptStatus.OK)  return res.StringResult;
            return null;
        }

        /// <summary>
        /// 获取用户输入的矩形角点区域
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="basePoint">The first point.</param>
        /// <returns>The point.</returns>
        public static Point3d getCorner(string message, Point3d basePoint) {
            var opt = new PromptCornerOptions(message, basePoint) { AllowNone = true };
            var res = GoatDB.ed.GetCorner(opt);
            if (res.Status == PromptStatus.OK)  return res.Value;
            return Constant.NullPoint3d;
        }

        /// <summary>
        /// 提示用户输入关键字   public PromptKeywordOptions(string messageAndKeywords, string globalKeywords)
        /// </summary>
        /// <param name="message">用户输入命令后的提示信息</param>
        /// <param name="keywords">The keywords.</param>
        /// <param name="defaultIndex">The default index.</param>
        /// <returns>The keyword result.</returns>
        public static string getKeywords(string messageAndKeywords, string globalKeywords,bool allowNone = false) {
            PromptKeywordOptions opt = new PromptKeywordOptions(messageAndKeywords,globalKeywords) { AllowNone = allowNone };
            var res = GoatDB.ed.GetKeywords(opt);
            if (res.Status == PromptStatus.OK) return res.StringResult;
            return null;
        }

        public static string getKeywords(string message, string[] keywords, int defaultIndex = 0,bool allowNone = false) {
            PromptKeywordOptions opt = new PromptKeywordOptions(message) { AllowNone = allowNone };
            keywords.ForEach(key => opt.Keywords.Add(key));
            opt.Keywords.Default = keywords[defaultIndex];
            var res = GoatDB.ed.GetKeywords(opt);
            if (res.Status == PromptStatus.OK) return res.StringResult;
            return null;
        }

        /// <summary>
        /// 提示用户选择单个实体
        /// </summary>
        /// <param name="msg">用户输入命令后的提示信息</param>
        /// <returns>实体对象</returns>
        public static Entity getEntity(string msg){
            PromptEntityResult ent = GoatDB.ed.GetEntity(msg);
            if (ent.Status == PromptStatus.OK) {
                return ent.ObjectId.QOpenForRead<Entity>();
            }
            return null;
        }

        public static ObjectId getEntityId(string msg){
            PromptEntityResult ent = GoatDB.ed.GetEntity(msg);
            if (ent.Status == PromptStatus.OK)  return ent.ObjectId;
            return ObjectId.Null;
        }

        public static ObjectId getEntityId(string message, Type allowedType, bool exactMatch = true)  {
            PromptEntityOptions opt = new PromptEntityOptions(message);
            // 设置用户选择指定类型之外的图形是的提示信息   此行代码必须在 AddAllowedClass 之前 否则报错
            opt.SetRejectMessage("Allowed type: " + allowedType.Name);
            // 设置 允许用户选择的图形类型  eg：只允许选择直线类型
            opt.AddAllowedClass(allowedType, exactMatch);
            PromptEntityResult res = GoatDB.ed.GetEntity(opt);
            if (res.Status == PromptStatus.OK) return res.ObjectId;
            return ObjectId.Null;
        }

        /// <summary>
        /// 设置选中
        /// </summary>
        /// <param name="entityIds"> 待选中的实体id</param>
        public static void SetPickSet(ObjectId[] entityIds) {
            GoatDB.ed.SetImpliedSelection(entityIds);
        }

        /// <summary>
        /// Highlights entities.
        /// </summary>
        /// <param name="entityIds">The entity IDs.</param>
        public static void HighlightObjects(IEnumerable<ObjectId> entityIds){
            entityIds.QForEach<Entity>(entity => entity.Highlight());
        }
        /// <summary>
        /// Unhighlights entities.
        /// </summary>
        /// <param name="entityIds">The entity IDs.</param>
        public static void UnhighlightObjects(IEnumerable<ObjectId> entityIds){
            entityIds.QForEach<Entity>(entity => entity.Unhighlight());
        }


    }
}