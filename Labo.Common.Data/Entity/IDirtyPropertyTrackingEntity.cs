namespace Labo.Common.Data.Entity
{
    public interface IDirtyPropertyTrackingEntity<out TEntity>
        where TEntity : class
    {
        bool IsDirty();

        bool IsDirtyTrackingEnabled();

        void SetDirtyTracking(bool enabled);

        string[] GetDirtyPropertyNames();

        void ClearDirtyProperties();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        TEntity GetEntity();
    }
}