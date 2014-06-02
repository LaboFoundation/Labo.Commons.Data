// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirtyPropertyTrackingEntity.cs" company="Labo">
//   The MIT License (MIT)
//   
//   Copyright (c) 2014 Bora Akgun
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to
//   use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
//   the Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary>
//   Defines the DirtyPropertyTrackingEntity type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Labo.Common.Data.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;

    using Labo.Common.Utils;

    [Serializable]
    public abstract class DirtyPropertyTrackingEntity<TEntity> : INotifyPropertyChanged, IDirtyPropertyTrackingEntity<TEntity>
        where TEntity : class
    {
        private readonly HashSet<string> m_DirtyPropertyNames = new HashSet<string>();

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private bool m_EnableDirtyTracking;

        protected DirtyPropertyTrackingEntity()
        {
            PropertyChanged += DirtyPropertyTrackingEntityPropertyChanged;
        }

        public virtual bool IsDirty()
        {
            return m_DirtyPropertyNames.Count != 0;
        }

        public virtual string[] GetDirtyPropertyNames()
        {
            return m_DirtyPropertyNames.ToArray();
        }

        public virtual void ClearDirtyProperties()
        {
            m_DirtyPropertyNames.Clear();
        }

        private void DirtyPropertyTrackingEntityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!m_EnableDirtyTracking)
            {
                return;
            }

            m_DirtyPropertyNames.Add(e.PropertyName);
        }

        public virtual void SetDirty<TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(LinqUtils.GetMemberName(expression)));
            }
        }

        public bool IsDirtyTrackingEnabled()
        {
            return m_EnableDirtyTracking;
        }

        public void SetDirtyTracking(bool enabled)
        {
            m_EnableDirtyTracking = enabled;
        }

        public abstract TEntity GetEntity();
    }
}