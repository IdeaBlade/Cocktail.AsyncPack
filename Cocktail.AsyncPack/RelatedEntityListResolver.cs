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

using System.Collections.Generic;
using System.Threading.Tasks;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    public class RelatedEntityListResolver
    {
        internal RelatedEntityListResolver()
        {
        }

        public static RelatedEntityListResolver<T> For<T>(IEnumerable<T> relatedEntityList) where T : class
        {
            return new RelatedEntityListResolver<T>(relatedEntityList);
        }
    }

    public class RelatedEntityListResolver<T> : RelatedEntityListResolver, IAwaitable<IEnumerable<T>> where T : class
    {
        private TaskCompletionSource<IEnumerable<T>> _tcs;

        internal RelatedEntityListResolver(IEnumerable<T> relatedEntityList)
        {
            RelatedEntityList = relatedEntityList;
        }

        public IEnumerable<T> RelatedEntityList { get; private set; }

        #region IAwaitable<IEnumerable<T>> Members

        public Task<IEnumerable<T>> AsTask()
        {
            var relatedEntityList = RelatedEntityList as RelatedEntityList<T>;
            if (relatedEntityList == null || !relatedEntityList.IsPendingEntityList)
                return Task.Factory.StartNew(() => RelatedEntityList);

            _tcs = new TaskCompletionSource<IEnumerable<T>>();
            relatedEntityList.PendingEntityListResolved += OnPendingEntityListResolved;
            return _tcs.Task;
        }

        Task IAwaitable.AsTask()
        {
            return AsTask();
        }

        #endregion

        private void OnPendingEntityListResolved(object sender, PendingEntityListResolvedEventArgs<T> args)
        {
            var relatedEntityList = (RelatedEntityList<T>)sender;
            relatedEntityList.PendingEntityListResolved -= OnPendingEntityListResolved;

            // TODO: What if navigation gets cancelled or fails.
            _tcs.SetResult(args.ResolvedEntities);
        }
    }
}