using Postulate.Mvc.Extensions;
using Postulate.Orm.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Postulate.Mvc.Helpers
{
    public class GridEditor<TRecord, TKey> where TRecord : Record<TKey>
    {
        private TRecord _dataRow = null;
        private TKey _rowId = default(TKey);
        private string _namePrefix = null;
        private List<string> _propertyNames = new List<string>();
        private List<string> _rowHiddenFields = new List<string>();
        private string _trID = null;
        private object _defaults = null;
        private Dictionary<string, SelectList> _selectLists = new Dictionary<string, SelectList>();
        private string _newRowContext = null;

        public GridEditor(string namePrefix = "", object defaults = null)
        {
            _defaults = defaults;
            _namePrefix = namePrefix;
            InsertFunction = "DataGridInsertRow";
            UpdateFunction = "DataGridSaveRow";
            DeleteFunction = "DataGridDeleteRow";
        }

        public string OnEditCallback { get; set; }
        public string OnSaveCallback { get; set; }
        public string OnActionCompleteCallback { get; set; }
        public string[] HiddenFields { get; set; }
        public string InsertFunction { get; set; }
        public string UpdateFunction { get; set; }
        public string DeleteFunction { get; set; }
        public bool UseAjax { get; set; }
        public bool PassDefaultsOnUpdate { get; set; }

        public void AddSelectList(string name, SelectList selectList)
        {
            _selectLists.Add(name, selectList);
        }

        public string RowID(TRecord dataRow)
        {
            _newRowContext = null;
            _dataRow = dataRow;
            _rowId = dataRow.Id;
            _trID = ControlId("row");
            return _trID;
        }

        public string NewRowID(string context = null)
        {
            _newRowContext = context;
            _dataRow = null;
            _rowId = default(TKey);
            _trID = ControlId("row");
            return _trID;
        }

        public MvcHtmlString Hidden<TValue>(Expression<Func<TRecord, TValue>> expression)
        {
            string propertyName = PropertyNameFromLambda(expression);
            if (!_propertyNames.Contains(propertyName))
            {
                _propertyNames.Add(propertyName);
                _rowHiddenFields.Add(propertyName);
            }

            TagBuilder hidden = new TagBuilder("input");
            hidden.MergeAttribute("type", "hidden");
            hidden.MergeAttribute("name", propertyName);
            hidden.MergeAttribute("id", ControlId(propertyName));

            if (IsSavedRow(_rowId))
            {
                var function = expression.Compile();
                var value = function.Invoke(_dataRow);
                hidden.MergeAttribute("value", value.ToString());
            }

            return MvcHtmlString.Create(hidden.ToString(TagRenderMode.SelfClosing));
        }

        public MvcHtmlString CheckBox<TValue>(Expression<Func<TRecord, TValue>> expression, string yesValue = "Yes", string noValue = "No", bool yesValueIsLabel = false, object htmlAttributes = null)
        {
            string propertyName = PropertyNameFromLambda(expression);
            if (!_propertyNames.Contains(propertyName)) _propertyNames.Add(propertyName);

            TagRenderMode closing = TagRenderMode.SelfClosing;
            /*TagRenderMode closing = (yesValueIsLabel) ? TagRenderMode.Normal : TagRenderMode.SelfClosing;

			TagBuilder chk = (yesValueIsLabel) ?
				new TagBuilder("label") { InnerHtml = CheckBoxInner(htmlAttributes, propertyName).ToString(TagRenderMode.SelfClosing) + yesValue } :
				CheckBoxInner(htmlAttributes, propertyName);*/

            TagBuilder chk = CheckBoxInner(htmlAttributes, propertyName);

            if (IsSavedRow(_rowId))
            {
                var function = expression.Compile();
                var value = function.Invoke(_dataRow);
                string output = noValue;
                if (Convert.ToBoolean(value))
                {
                    output = yesValue;
                    chk.MergeAttribute("checked", "checked");
                }

                return BuildSpans(propertyName, chk.ToString(closing), output);
            }
            else
            {
                object isChecked = false;
                if (DefaultValueExists(propertyName, out isChecked) && Convert.ToBoolean(isChecked)) chk.MergeAttribute("checked", "checked");
                return MvcHtmlString.Create(chk.ToString(closing));
            }
        }

        private bool IsSavedRow(TKey rowId)
        {
            return !rowId.Equals(default(TKey));
        }

        private bool DefaultValueExists(string propertyName, out object value)
        {
            value = null;
            if (_defaults == null) return false;

            PropertyInfo[] props = _defaults.GetType().GetProperties();
            PropertyInfo pi = props.SingleOrDefault(p => p.Name.Equals(propertyName));
            if (pi != null)
            {
                value = pi.GetValue(_defaults, null);
                return true;
            }

            return false;
        }

        private TagBuilder CheckBoxInner(object htmlAttributes, string propertyName)
        {
            TagBuilder chk = new TagBuilder("input");
            chk.MergeAttribute("type", "checkbox");
            chk.MergeAttribute("name", propertyName);
            chk.MergeAttribute("id", ControlId(propertyName));
            if (htmlAttributes != null) chk.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            return chk;
        }

        public MvcHtmlString DropDownList<TValue>(Expression<Func<TRecord, TValue>> expression, string selectListName, object htmlAttributes = null)
        {
            return DropDownList<TValue>(expression, _selectLists[selectListName], htmlAttributes);
        }

        public MvcHtmlString DropDownList<TValue>(Expression<Func<TRecord, TValue>> expression, SelectList selectList, object htmlAttributes = null)
        {
            string propertyName = PropertyNameFromLambda(expression);
            if (!_propertyNames.Contains(propertyName)) _propertyNames.Add(propertyName);

            TagBuilder select = new TagBuilder("select");
            select.MergeAttribute("name", propertyName);
            select.MergeAttribute("id", ControlId(propertyName));
            if (htmlAttributes != null) select.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), true);

            TagBuilder blankOption = new TagBuilder("option");
            blankOption.SetInnerText("(select)");
            select.InnerHtml += blankOption.ToString(TagRenderMode.Normal);

            if (IsSavedRow(_rowId))
            {
                var function = expression.Compile();
                var value = function.Invoke(_dataRow);
                string displayValue = value?.ToString();

                select.BuildSelectListOptions(selectList, null, value, out displayValue);

                return BuildSpans(propertyName, select.ToString(TagRenderMode.Normal), displayValue);
            }
            else
            {
                object defaultValue;
                DefaultValueExists(propertyName, out defaultValue);

                select.BuildSelectListOptions(selectList, null, defaultValue);

                return MvcHtmlString.Create(select.ToString(TagRenderMode.Normal));
            }
        }

        /* doesn't work because JavaScript functions don't know how to deal with this
		public MvcHtmlString Hidden<TValue>(Expression<Func<TModel, TValue>> expression)
		{
			string propertyName = PropertyNameFromLambda(expression);
			if (!_propertyNames.Contains(propertyName)) _propertyNames.Add(propertyName);

			TagBuilder hidden = new TagBuilder("input");
			hidden.MergeAttribute("type", "hidden");
			hidden.MergeAttribute("name", propertyName);
			hidden.MergeAttribute("id", ControlID(propertyName));

			if (_rowID != 0)
			{
				var function = expression.Compile();
				object rawValue = function.Invoke(_dataRow);
				hidden.MergeAttribute("value", rawValue.ToString());
			}
			else
			{
				object defaultText;
				if (DefaultValueExists(propertyName, out defaultText)) hidden.MergeAttribute("value", defaultText.ToString());
			}

			return MvcHtmlString.Create(hidden.ToString(TagRenderMode.SelfClosing));
		}*/

        public MvcHtmlString Label<TValue>(Expression<Func<TRecord, TValue>> expression, string format = null, object htmlAttributes = null)
        {
            string propertyName = PropertyNameFromLambda(expression);

            var function = expression.Compile();
            object rawValue = function.Invoke(_dataRow);
            string formattedValue = (!string.IsNullOrEmpty(format) && rawValue != null) ? string.Format("{0:" + format + "}", rawValue) : rawValue.ToString();

            TagBuilder span = new TagBuilder("span");

            TagBuilder label = new TagBuilder("label");
            label.SetInnerText(formattedValue);

            TagBuilder hidden = new TagBuilder("input");
            hidden.MergeAttribute("type", "hidden");
            hidden.MergeAttribute("name", propertyName);
            hidden.MergeAttribute("id", ControlId(propertyName));
            if (rawValue != null) hidden.MergeAttribute("value", rawValue.ToString());

            span.InnerHtml += label.ToString(TagRenderMode.Normal);
            span.InnerHtml += hidden.ToString(TagRenderMode.SelfClosing);

            return MvcHtmlString.Create(span.ToString(TagRenderMode.Normal));
        }

        public MvcHtmlString TextBox<TValue>(Expression<Func<TRecord, TValue>> expression, object htmlAttributes = null)
        {
            string propertyName = PropertyNameFromLambda(expression);
            if (!_propertyNames.Contains(propertyName)) _propertyNames.Add(propertyName);

            TagBuilder textBox = new TagBuilder("input");
            textBox.MergeAttribute("type", "text");
            textBox.MergeAttribute("name", propertyName);
            textBox.MergeAttribute("id", ControlId(propertyName));
            if (htmlAttributes != null) textBox.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            string displayValue = string.Empty;

            if (IsSavedRow(_rowId))
            {
                var function = expression.Compile();
                object rawValue = function.Invoke(_dataRow);

                if (rawValue != null)
                {
                    displayValue = rawValue.ToString();

                    PropertyInfo pi = _dataRow.GetType().GetProperty(propertyName);
                    object[] attr = pi.GetCustomAttributes(typeof(DisplayFormatAttribute), false);
                    if (attr.Length == 1)
                    {
                        DisplayFormatAttribute format = (DisplayFormatAttribute)attr[0];
                        displayValue = string.Format(format.DataFormatString, rawValue);
                    }
                }

                textBox.MergeAttribute("value", displayValue);
                return BuildSpans(propertyName, textBox.ToString(TagRenderMode.SelfClosing), displayValue);
            }
            else
            {
                object defaultText;
                if (DefaultValueExists(propertyName, out defaultText)) textBox.MergeAttribute("value", defaultText.ToString());
                return MvcHtmlString.Create(textBox.ToString(TagRenderMode.SelfClosing));
            }
        }

        public MvcHtmlString TextArea<TValue>(Expression<Func<TRecord, TValue>> expression, object htmlAttributes = null)
        {
            string propertyName = PropertyNameFromLambda(expression);
            if (!_propertyNames.Contains(propertyName)) _propertyNames.Add(propertyName);

            TagBuilder textArea = new TagBuilder("textarea");
            textArea.MergeAttribute("name", propertyName);
            textArea.MergeAttribute("id", ControlId(propertyName));
            if (htmlAttributes != null) textArea.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            string displayValue = string.Empty;

            if (IsSavedRow(_rowId))
            {
                var function = expression.Compile();
                object rawValue = function.Invoke(_dataRow);

                if (rawValue != null)
                {
                    displayValue = rawValue.ToString();

                    PropertyInfo pi = _dataRow.GetType().GetProperty(propertyName);
                    object[] attr = pi.GetCustomAttributes(typeof(DisplayFormatAttribute), false);
                    if (attr.Length == 1)
                    {
                        DisplayFormatAttribute format = (DisplayFormatAttribute)attr[0];
                        displayValue = string.Format(format.DataFormatString, rawValue);
                    }
                }

                textArea.SetInnerText(displayValue);
                return BuildSpans(propertyName, textArea.ToString(TagRenderMode.Normal), displayValue);
            }
            else
            {
                object defaultText;
                if (DefaultValueExists(propertyName, out defaultText)) textArea.SetInnerText(defaultText.ToString());
                return MvcHtmlString.Create(textArea.ToString(TagRenderMode.Normal));
            }
        }

        public string ControlId(string propertyName)
        {
            string result = ((_namePrefix.Length > 0) ? _namePrefix + "-" : "") + propertyName + "-" + _rowId.ToString();

            if (!string.IsNullOrEmpty(_newRowContext)) result += _newRowContext;

            return result;
        }

        private string PropertyNameFromLambda(Expression expression)
        {
            // thanks to http://odetocode.com/blogs/scott/archive/2012/11/26/why-all-the-lambdas.aspx
            // thanks to http://stackoverflow.com/questions/671968/retrieving-property-name-from-lambda-expression

            LambdaExpression le = expression as LambdaExpression;
            if (le == null) throw new ArgumentException("expression");

            MemberExpression me = null;
            if (le.Body.NodeType == ExpressionType.Convert)
            {
                me = ((UnaryExpression)le.Body).Operand as MemberExpression;
            }
            else if (le.Body.NodeType == ExpressionType.MemberAccess)
            {
                me = le.Body as MemberExpression;
            }

            if (me == null) throw new ArgumentException("expression");

            return me.Member.Name;
        }

        private MvcHtmlString BuildSpans(string propertyName, string editMarkup, string displayMarkup)
        {
            TagBuilder spanEdit = new TagBuilder("span");
            spanEdit.MergeAttribute("id", "edit-" + ControlId(propertyName));
            spanEdit.MergeAttribute("style", "display:none");
            spanEdit.InnerHtml = editMarkup;

            TagBuilder spanDisplay = new TagBuilder("span");
            spanDisplay.MergeAttribute("id", "display-" + ControlId(propertyName));
            spanDisplay.InnerHtml = displayMarkup;

            return MvcHtmlString.Create(spanEdit.ToString(TagRenderMode.Normal) + spanDisplay.ToString(TagRenderMode.Normal));
        }

        public MvcHtmlString Controls(string validationFunction = null)
        {
            return Controls(true, true, validationFunction);
        }

        public MvcHtmlString Controls(bool allowEdit, bool allowDelete, string validationFunction = null)
        {
            string editFieldIDs = "[" + string.Join(",", _propertyNames.Where(s => !_rowHiddenFields.Contains(s)).Select(s => "'" + ControlId(s) + "'")) + "]";
            string editFieldNames = "[" + string.Join(",", _propertyNames.Where(s => !_rowHiddenFields.Contains(s)).Select(s => "'" + s + "'")) + "]";

            string saveFieldIDs = (HiddenFields != null) ?
                "[" + string.Join(",", _propertyNames.Concat(HiddenFields).Select(s => "'" + ControlId(s) + "'")) + "]" :
                "[" + string.Join(",", _propertyNames.Select(s => "'" + ControlId(s) + "'")) + "]";

            string saveFieldNames = (HiddenFields != null) ?
                "[" + string.Join(",", _propertyNames.Concat(HiddenFields).Select(s => "'" + s + "'")) + "]" :
                "[" + string.Join(",", _propertyNames.Select(s => "'" + s + "'")) + "]";

            string jsArgsEdit = "'" + _trID + "', " + editFieldIDs + ", " + _rowId.ToString();
            if (!UseAjax) jsArgsEdit += ", '" + SaveFormID() + "', " + (!string.IsNullOrEmpty(OnEditCallback) ? OnEditCallback : "null");

            string jsArgsSave = saveFieldIDs + ", " + saveFieldNames;
            if (!UseAjax) jsArgsSave += ", " + (!string.IsNullOrEmpty(validationFunction) ? validationFunction : "null") + ", " + (!string.IsNullOrEmpty(OnActionCompleteCallback) ? OnActionCompleteCallback : "null");

            string jsArgsInsert = jsArgsSave;
            Dictionary<string, object> hiddenDefaults;
            if (UseAjax && HasHiddenDefaults(out hiddenDefaults))
            {
                jsArgsInsert += ", [" + string.Join(",", hiddenDefaults.Select(kvp => "'" + kvp.Key + "'")) + "], [" + string.Join(",", hiddenDefaults.Select(kvp => kvp.Value)) + "]";

                if (PassDefaultsOnUpdate)
                {
                    jsArgsSave += ", [" + string.Join(",", hiddenDefaults.Select(kvp => "'" + kvp.Key + "'")) + "], [" + string.Join(",", hiddenDefaults.Select(kvp => kvp.Value)) + "]";
                }
            }

            if (IsSavedRow(_rowId))
            {
                // edit, delete links
                TagBuilder spanClean = new TagBuilder("span");
                spanClean.MergeAttribute("id", _trID + "-clean");

                if (allowEdit)
                {
                    TagBuilder aEdit = new TagBuilder("a");
                    aEdit.MergeAttribute("href", "javascript:DataGridEditRow(" + jsArgsEdit + ")");
                    aEdit.SetInnerText("edit");
                    spanClean.InnerHtml = aEdit.ToString(TagRenderMode.Normal);
                }

                if (allowDelete)
                {
                    if (allowEdit) spanClean.InnerHtml += "&nbsp;|&nbsp;";

                    TagBuilder aDelete = new TagBuilder("a");
                    aDelete.SetInnerText("delete");
                    if (!UseAjax)
                    {
                        aDelete.MergeAttribute("href", "javascript:" + DeleteFunction + "('" + DeleteFormID() + "', " + _rowId.ToString() + ")");
                    }
                    else
                    {
                        aDelete.MergeAttribute("href", "javascript:" + DeleteFunction + "(" + _rowId.ToString() + ")");
                    }
                    spanClean.InnerHtml += aDelete.ToString(TagRenderMode.Normal);
                }

                // save, cancel links
                TagBuilder spanDirty = new TagBuilder("span");
                spanDirty.MergeAttribute("id", _trID + "-dirty");
                spanDirty.MergeAttribute("style", "display:none");

                TagBuilder aSave = new TagBuilder("a");
                if (!UseAjax)
                {
                    aSave.MergeAttribute("href", "javascript:" + UpdateFunction + "(" + jsArgsSave + ")");
                }
                else
                {
                    aSave.MergeAttribute("href", "javascript:" + UpdateFunction + "(" + _rowId.ToString() + ", " + jsArgsSave + ")");
                }

                aSave.SetInnerText("save");
                spanDirty.InnerHtml = aSave.ToString(TagRenderMode.Normal);

                TagBuilder aCancel = new TagBuilder("a");
                aCancel.MergeAttribute("href", "javascript:DataGridCancelEdit(" + (!string.IsNullOrEmpty(OnActionCompleteCallback) ? OnActionCompleteCallback : "null") + ")");
                aCancel.SetInnerText("cancel");
                spanDirty.InnerHtml += "&nbsp;|&nbsp;" + aCancel.ToString(TagRenderMode.Normal);

                return MvcHtmlString.Create(spanClean.ToString(TagRenderMode.Normal) + spanDirty.ToString(TagRenderMode.Normal).Replace("&quot;", "\""));
            }
            else
            {
                TagBuilder aInsert = new TagBuilder("a");
                if (!UseAjax)
                {
                    aInsert.MergeAttribute("href", "javascript:" + InsertFunction + "('" + SaveFormID() + "', " + jsArgsSave + ")");
                }
                else
                {
                    aInsert.MergeAttribute("href", "javascript:" + InsertFunction + "(" + jsArgsInsert + ")");
                }
                aInsert.SetInnerText("add record");

                return MvcHtmlString.Create(aInsert.ToString(TagRenderMode.Normal));
            }
        }

        private bool HasHiddenDefaults(out Dictionary<string, object> hiddenDefaults)
        {
            hiddenDefaults = null;

            if (_defaults != null)
            {
                var results = _defaults.ToDictionary();
                hiddenDefaults = results.Where(kvp => !_propertyNames.Contains(kvp.Key)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                return (results.Count() > 0);
            }

            return false;
        }

        public MvcHtmlString SaveForms(string saveUrl, string deleteUrl, string returnUrl = null)
        {
            TagBuilder formSpan = new TagBuilder("span");
            formSpan.MergeAttribute("style", "display:none");

            var url = new UrlHelper(HttpContext.Current.Request.RequestContext);

            // save form
            TagBuilder saveForm = new TagBuilder("form");
            saveForm.MergeAttribute("action", url.Content(saveUrl));
            saveForm.MergeAttribute("method", "post");
            saveForm.MergeAttribute("id", SaveFormID());
            if (!string.IsNullOrEmpty(returnUrl)) AddReturnUrlField(saveForm, returnUrl);
            foreach (var prop in _propertyNames)
            {
                TagBuilder hidden = new TagBuilder("input");
                hidden.MergeAttribute("type", "hidden");
                hidden.MergeAttribute("name", prop);
                saveForm.InnerHtml += hidden.ToString(TagRenderMode.SelfClosing);
            }

            if (HiddenFields != null)
            {
                foreach (string fieldName in HiddenFields)
                {
                    TagBuilder hidden = new TagBuilder("input");
                    hidden.MergeAttribute("type", "hidden");
                    hidden.MergeAttribute("name", fieldName);
                    saveForm.InnerHtml += hidden.ToString(TagRenderMode.SelfClosing);
                }
            }

            if (_defaults != null)
            {
                PropertyInfo[] props = _defaults.GetType().GetProperties();
                foreach (PropertyInfo pi in props)
                {
                    TagBuilder defaultValue = new TagBuilder("input");
                    defaultValue.MergeAttribute("type", "hidden");
                    defaultValue.MergeAttribute("name", pi.Name);
                    defaultValue.MergeAttribute("value", pi.GetValue(_defaults, null).ToString());
                    saveForm.InnerHtml += defaultValue.ToString(TagRenderMode.SelfClosing);
                }
            }

            TagBuilder hiddenRowID = new TagBuilder("input");
            hiddenRowID.MergeAttribute("type", "hidden");
            hiddenRowID.MergeAttribute("name", "ID");
            saveForm.InnerHtml += hiddenRowID.ToString(TagRenderMode.SelfClosing);

            formSpan.InnerHtml += saveForm.ToString(TagRenderMode.Normal);

            // delete form
            TagBuilder deleteForm = new TagBuilder("form");
            deleteForm.MergeAttribute("action", url.Content(deleteUrl));
            deleteForm.MergeAttribute("method", "post");
            deleteForm.MergeAttribute("id", DeleteFormID());
            if (!string.IsNullOrEmpty(returnUrl)) AddReturnUrlField(deleteForm, returnUrl);
            deleteForm.InnerHtml += hiddenRowID.ToString(TagRenderMode.SelfClosing);

            formSpan.InnerHtml += deleteForm.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(formSpan.ToString(TagRenderMode.Normal));
        }

        private void AddReturnUrlField(TagBuilder formTag, string returnUrl)
        {
            TagBuilder hidden = new TagBuilder("input");
            hidden.MergeAttribute("type", "hidden");
            hidden.MergeAttribute("name", "returnUrl");
            hidden.MergeAttribute("value", returnUrl);
            formTag.InnerHtml += hidden.ToString(TagRenderMode.SelfClosing);
        }

        private string DeleteFormID()
        {
            return "deleteForm-" + _namePrefix;
        }

        private string SaveFormID()
        {
            return "saveForm-" + _namePrefix;
        }
    }
}