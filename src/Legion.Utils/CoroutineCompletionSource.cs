using AwaitableCoroutine;
using System;

namespace Legion.Utils
{
    public class CoroutineCompletionSource
    {
        class CustomCoroutine : Coroutine
        {
            private bool _completionRequested = false;
            private bool _cancellationRequested = false;

            protected override void OnMoveNext()
            {
                if (_cancellationRequested)
                {
                    Cancel();
                }
                else if (_completionRequested)
                {
                    Complete();
                }
            }

            public void SetCompleted()
            {
                if (_completionRequested || _cancellationRequested)
                    throw new InvalidOperationException("Coroutine already completed");

                _completionRequested = true;
            }

            public void SetCanceled()
            {
                if (_completionRequested || _cancellationRequested)
                    throw new InvalidOperationException("Coroutine already completed");

                _cancellationRequested = true;
            }
        }

        private readonly CustomCoroutine _coroutine;
        public Coroutine Coroutine => _coroutine;

        public CoroutineCompletionSource()
        {
            _coroutine = new CustomCoroutine();
        }

        public CoroutineCompletionSource(ICoroutineRunner runner)
        {
            _coroutine = runner.Context(() => new CustomCoroutine());
        }

        public void SetCompleted()
        {
            _coroutine.SetCompleted();
        }

        public void SetCanceled()
        {
            _coroutine.SetCanceled();
        }
    }
}
