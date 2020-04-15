﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using OpenQA.Selenium;

namespace Atata
{
    /// <summary>
    /// Represents the list of controls of <typeparamref name="TItem"/> type.
    /// </summary>
    /// <typeparam name="TItem">The type of the item control.</typeparam>
    /// <typeparam name="TOwner">The type of the owner page object.</typeparam>
    public class ControlList<TItem, TOwner> : UIComponentPart<TOwner>, IDataProvider<IEnumerable<TItem>, TOwner>, IEnumerable<TItem>, ISupportsMetadata
        where TItem : Control<TOwner>
        where TOwner : PageObject<TOwner>
    {
        private string itemComponentTypeName;

        [Obsolete("This property is not used internally anymore, no sense to use it.")] // Obsolete since v1.5.0.
        protected ControlDefinitionAttribute ItemDefinition =>
            (ControlDefinitionAttribute)Metadata.ComponentDefinitionAttribute;

        [Obsolete("This property is not used internally anymore, no sense to use it.")] // Obsolete since v1.5.0.
        protected FindAttribute ItemFindAttribute =>
            ResolveItemFindAttribute();

        protected string ItemComponentTypeName =>
            itemComponentTypeName ?? (itemComponentTypeName = UIComponentResolver.ResolveControlTypeName(Metadata));

        /// <summary>
        /// Gets the assertion verification provider that has a set of verification extension methods.
        /// </summary>
        public DataVerificationProvider<IEnumerable<TItem>, TOwner> Should => new DataVerificationProvider<IEnumerable<TItem>, TOwner>(this);

        /// <summary>
        /// Gets the expectation verification provider that has a set of verification extension methods.
        /// </summary>
        public DataVerificationProvider<IEnumerable<TItem>, TOwner> ExpectTo => Should.Using<ExpectationVerificationStrategy>();

        /// <summary>
        /// Gets the waiting verification provider that has a set of verification extension methods.
        /// Uses <see cref="AtataContext.WaitingTimeout"/> and <see cref="AtataContext.WaitingRetryInterval"/> of <see cref="AtataContext.Current"/> for timeout and retry interval.
        /// </summary>
        public DataVerificationProvider<IEnumerable<TItem>, TOwner> WaitTo => Should.Using<WaitingVerificationStrategy>();

        /// <summary>
        /// Gets the <see cref="DataProvider{TData, TOwner}"/> instance for the controls count.
        /// </summary>
        public DataProvider<int, TOwner> Count => Component.GetOrCreateDataProvider($"{ComponentPartName} count", GetCount);

        /// <summary>
        /// Gets the <see cref="DataProvider{TData, TOwner}"/> instance for the controls contents.
        /// </summary>
        public DataProvider<IEnumerable<string>, TOwner> Contents => Component.GetOrCreateDataProvider($"{ComponentPartName} contents", GetContents);

        UIComponent IDataProvider<IEnumerable<TItem>, TOwner>.Component => (UIComponent)Component;

        protected string ProviderName => $"{ComponentPartName}";

        string IDataProvider<IEnumerable<TItem>, TOwner>.ProviderName => ProviderName;

        TOwner IDataProvider<IEnumerable<TItem>, TOwner>.Owner => Component.Owner;

        TermOptions IDataProvider<IEnumerable<TItem>, TOwner>.ValueTermOptions { get; }

        IEnumerable<TItem> IDataProvider<IEnumerable<TItem>, TOwner>.Value => GetAll();

        UIComponentMetadata ISupportsMetadata.Metadata
        {
            get { return Metadata; }
            set { Metadata = value; }
        }

        public UIComponentMetadata Metadata { get; private set; }

        Type ISupportsMetadata.ComponentType
        {
            get { return typeof(TItem); }
        }

        /// <summary>
        /// Gets the control at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to get.</param>
        /// <returns>The item at the specified index.</returns>
        public TItem this[int index]
        {
            get
            {
                index.CheckIndexNonNegative();

                return GetItemByIndex(index);
            }
        }

        /// <summary>
        /// Gets the control that matches the conditions defined by the specified predicate expression.
        /// </summary>
        /// <param name="predicateExpression">The predicate expression to test each item.</param>
        /// <returns>The first item that matches the conditions of the specified predicate.</returns>
        public TItem this[Expression<Func<TItem, bool>> predicateExpression]
        {
            get
            {
                predicateExpression.CheckNotNull(nameof(predicateExpression));

                string itemName = UIComponentResolver.ResolveControlName<TItem, TOwner>(predicateExpression);

                return GetItem(itemName, predicateExpression);
            }
        }

        /// <summary>
        /// Gets the control that matches the specified XPath condition.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <param name="xPathCondition">
        /// The XPath condition.
        /// For example: <c>"@some-attr='some value'"</c>.</param>
        /// <returns>The first item that matches the XPath condition.</returns>
        public TItem GetByXPathCondition(string itemName, string xPathCondition)
        {
            return GetItemByInnerXPath(itemName, xPathCondition);
        }

        private FindAttribute ResolveItemFindAttribute()
        {
            return new FindControlListItemAttribute();
        }

        /// <summary>
        /// Gets the controls count.
        /// </summary>
        /// <returns>The count of controls.</returns>
        protected virtual int GetCount()
        {
            return GetItemElements().Count;
        }

        /// <summary>
        /// Gets the controls contents.
        /// </summary>
        /// <returns>The contents of controls.</returns>
        protected virtual IEnumerable<string> GetContents()
        {
            return GetAll().Select(x => (string)x.Content);
        }

        protected TItem GetItemByIndex(int index)
        {
            string itemName = (index + 1).Ordinalize();

            return CreateItem(itemName, new FindByIndexAttribute(index));
        }

        protected TItem GetItemByInnerXPath(string itemName, string xPath)
        {
            return CreateItem(itemName, new FindByInnerXPathAttribute(xPath));
        }

        protected virtual TItem GetItem(string name, Expression<Func<TItem, bool>> predicateExpression)
        {
            var predicate = predicateExpression.Compile();

            ControlListScopeLocator scopeLocator = new ControlListScopeLocator(searchOptions =>
            {
                return GetItemElements(searchOptions).
                    Where(element => predicate(CreateItem(new DefinedScopeLocator(element), name)));
            });

            return CreateItem(scopeLocator, name);
        }

        /// <summary>
        /// Searches for the item that matches the conditions defined by the specified predicate expression
        /// and returns the zero-based index of the first occurrence.
        /// </summary>
        /// <param name="predicateExpression">The predicate expression to test each item.</param>
        /// <returns>The zero-based index of the first occurrence of item, if found; otherwise, <c>–1</c>.</returns>
        public DataProvider<int, TOwner> IndexOf(Expression<Func<TItem, bool>> predicateExpression)
        {
            predicateExpression.CheckNotNull(nameof(predicateExpression));

            string itemName = UIComponentResolver.ResolveControlName<TItem, TOwner>(predicateExpression);

            return Component.GetOrCreateDataProvider($"{ComponentPartName} index of \"{itemName}\" {ItemComponentTypeName}", () =>
            {
                return IndexOf(itemName, predicateExpression);
            });
        }

        protected virtual int IndexOf(string name, Expression<Func<TItem, bool>> predicateExpression)
        {
            var predicate = predicateExpression.Compile();

            return GetItemElements().
                Select((element, index) => new { Element = element, Index = index }).
                Where(x => predicate(CreateItem(new DefinedScopeLocator(x.Element), name))).
                Select(x => (int?)x.Index).
                FirstOrDefault() ?? -1;
        }

        [Obsolete("This method is not used anymore, no sense to invoke or override it.")] // Obsolete since v1.5.0.
        protected virtual By CreateItemBy()
        {
            FindAttribute itemFindAttribute = ResolveItemFindAttribute();
            itemFindAttribute.Properties.Metadata = Metadata;

            string outerXPath = itemFindAttribute.OuterXPath ?? ".//";

            By by = By.XPath($"{outerXPath}{ItemDefinition.ScopeXPath}").OfKind(ItemComponentTypeName);

            // TODO: Review/remake this Visibility processing.
            if (itemFindAttribute.Visibility == Visibility.Any)
                by = by.OfAnyVisibility();
            else if (itemFindAttribute.Visibility == Visibility.Hidden)
                by = by.Hidden();

            return by;
        }

        protected virtual TItem CreateItem(string name, params Attribute[] attributes)
        {
            var itemAttributes = new Attribute[] { new NameAttribute(name) }.Concat(
                attributes?.Concat(GetItemDeclaredAttributes()) ?? GetItemDeclaredAttributes());

            return CreateItem(itemAttributes);
        }

        protected TItem CreateItem(IScopeLocator scopeLocator, string name)
        {
            TItem item = CreateItem(name);

            if (scopeLocator is ControlListScopeLocator controlListScopeLocator)
                controlListScopeLocator.ElementName = item.ComponentFullName;

            item.ScopeLocator = scopeLocator;

            return item;
        }

        private TItem CreateItem(IEnumerable<Attribute> itemAttributes)
        {
            TItem control = Component.Controls.Create<TItem>(Metadata.Name, itemAttributes.ToArray());

            // TODO: Remove control removal.
            Component.Controls.Remove(control);
            return control;
        }

        protected virtual IEnumerable<Attribute> GetItemDeclaredAttributes()
        {
            yield return ResolveItemFindAttribute();

            foreach (var item in Metadata.DeclaredAttributes)
                yield return item;
        }

        /// <summary>
        /// Selects the specified data (property) set of each control.
        /// Data can be a sub-control, an instance of <see cref="DataProvider{TData, TOwner}"/>, etc.
        /// </summary>
        /// <typeparam name="TData">The type of the data.</typeparam>
        /// <param name="selector">The data selector.</param>
        /// <returns>An instance of <see cref="DataProvider{TData, TOwner}"/>.</returns>
        public DataProvider<IEnumerable<TData>, TOwner> SelectData<TData>(Expression<Func<TItem, TData>> selector)
        {
            string dataPathName = ObjectExpressionStringBuilder.ExpressionToString(selector);

            return Component.GetOrCreateDataProvider(
                $"\"{dataPathName}\" {ProviderName}",
                () => GetAll().Select(selector.Compile()));
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<TItem> GetEnumerator()
        {
            return GetAll().GetEnumerator();
        }

        protected virtual IEnumerable<TItem> GetAll()
        {
            return GetItemElements().
                Select((element, index) => CreateItem(new DefinedScopeLocator(element), (index + 1).Ordinalize())).
                ToArray();
        }

        [Obsolete("Use GetItemElements() instead.")] // Obsolete since v1.5.0.
        protected ReadOnlyCollection<IWebElement> GetItemElements(By itemBy)
        {
            return GetItemElements();
        }

        protected ReadOnlyCollection<IWebElement> GetItemElements(SearchOptions searchOptions = null)
        {
            TItem control = CreateItem(GetItemDeclaredAttributes());

            return control.ScopeLocator.GetElements(searchOptions).ToReadOnly();
        }
    }
}
