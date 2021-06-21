namespace MakeSimple.SharedKernel.Infrastructure.Attributes
{
    using Microsoft.AspNetCore.Mvc.Filters;
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    public class MemoryWatchAttribute : ActionFilterAttribute
    {
        public long Before { get; private set; }
        public long After { get; private set; }
        public long Change { get; private set; }

        public bool CalculateFromGC { get; set; } = false;

        public MemoryWatchAttribute()
        {
            // Do something here in the future?
        }

        public MemoryWatchAttribute(bool calculateFromGC)
        {
            this.CalculateFromGC = calculateFromGC;
        }

        #region Private methods
        private void CalculateMemory()
        {
            this.Change = this.After - this.Before;

            this.PrintUsage();
        }

        private void PrintUsage()
        {
            var now = DateTime.Now;
            Debug.WriteLine($"{now} [{nameof(this.Before)}] - Memory used: {this.Before}");
            Debug.WriteLine($"{now} [{nameof(this.After)}] - Memory used: {this.After}");
            Debug.WriteLine($"{now} [{nameof(this.Change)}] - Memory used: {this.Change}");
        }

        private void SetBefore()
        {
            if (this.CalculateFromGC)
            {
                this.Before = GC.GetTotalMemory(true);
            }
            else
            {
                this.Before = Process.GetCurrentProcess().PrivateMemorySize64;
            }
        }

        private void SetAfter()
        {
            if (this.CalculateFromGC)
            {
                this.After = GC.GetTotalMemory(true);
            }
            else
            {
                this.After = Process.GetCurrentProcess().PrivateMemorySize64;
            }
        }
        #endregion


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            this.SetBefore();
            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            this.SetAfter();
            this.CalculateMemory();

            base.OnActionExecuted(context);
        }

        public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Do something before the action executes.
            this.SetBefore();
            // next() calls the action method.
            await next();
            // Do something after the action executes.
            this.SetAfter();
            this.CalculateMemory();
        }
    }
}
