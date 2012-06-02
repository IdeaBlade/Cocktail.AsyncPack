// ====================================================================================================================
//   Copyright (c) 2012 IdeaBlade
// ====================================================================================================================
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//   OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//   OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// ====================================================================================================================
//   USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
//   http://cocktail.ideablade.com/licensing
// ====================================================================================================================

using System.Threading.Tasks;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    public class EntityResolver
    {
        internal EntityResolver()
        {
        }

        public static EntityResolver<T> For<T>(T entity) where T : class
        {
            return new EntityResolver<T>(entity);
        }
    }

    public class EntityResolver<T> : EntityResolver, IAwaitable<T> where T : class
    {
        private TaskCompletionSource<T> _tcs;

        internal EntityResolver(T entity)
        {
            Entity = entity;
        }

        public T Entity { get; private set; }

        #region IAwaitable<T> Members

        public Task<T> AsTask()
        {
            var entityAspect = EntityAspect.Wrap(Entity);
            if (!entityAspect.IsPendingEntity)
                return Task.Factory.StartNew(() => Entity);

            _tcs = new TaskCompletionSource<T>();
            entityAspect.PendingEntityResolved += OnPendingEntityResolved;
            return _tcs.Task;
        }

        Task IAwaitable.AsTask()
        {
            return AsTask();
        }

        #endregion

        private void OnPendingEntityResolved(object sender, PendingEntityResolvedEventArgs args)
        {
            var entityAspect = (EntityAspect)sender;
            entityAspect.PendingEntityResolved -= OnPendingEntityResolved;

            // TODO: What if navigation gets cancelled or fails.
            _tcs.SetResult((T)args.ResolvedEntity);
        }
    }
}