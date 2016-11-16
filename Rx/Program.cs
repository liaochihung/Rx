using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace Rx
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestFromEventPattern();

            //TestObservable();

            //var block = BlockingMethod();
            //block.Subscribe(Console.WriteLine);

            //var nonBlock = NonBlocking();
            //nonBlock.Subscribe(Console.WriteLine);

            //NonBlocking_event_driven();

            //TestUnfold();

            //var range = Observable.Range(10, 20);
            //range.Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));

            //TestObservableTimer();

            //TestWhere();

            //testDistinct();

            //testIgnoreElements();

            //testSkipAndTake();

            //testSkipWhile();

            //testSkipUntil();

            //testContains();

            //testAll();

            //testCount();

            //testAggregations();

            //testSelect();

            //testInterval();

            //testMaterialize();

            //testSelectMany();

            //testSelect1();

            //testToEnumerable();

            //TestStartAndReturn();

            //TestBuffer();

            //TestWindow();

            Console.WriteLine("press any key to exit.");
            Console.ReadKey();
        }

        private static void TestWindow()
        {
            var input = new[] {1, 2, 3, 4, 5}.ToObservable();
            int i = 0;
            input.Window(2).Subscribe(obs =>
            {
                int current = ++i;
                Console.WriteLine("Started observable {0}", current);
                obs.Subscribe(
                    item => Console.WriteLine("  {0}", item),
                    () => Console.WriteLine("ended observable {0}", current));
            });
        }

        private static void TestBuffer()
        {
            var input = new[] {1, 2, 3, 4, 5}.ToObservable();
            int i = 0;
            input.Buffer(2).Subscribe(x =>
            {
                Console.WriteLine("start");
                foreach (var i1 in x)
                {
                    Console.WriteLine("item value:{0}", i1);
                }
                Console.WriteLine("--------------------------");

                // code below can't pass compiler.
                //int current = ++i;
                //Console.WriteLine("Started observable {0}", current);
                //x.Subscribe(
                //    item => Console.WriteLine("   {0}", item),
                //    () => Console.WriteLine("ended observable {0}", current));
            });
        }

        private static void TestStartAndReturn()
        {
            Console.WriteLine("Current thread id: {0}",
                Thread.CurrentThread.ManagedThreadId);
            Add2NumbersAsync(2, 3)
                .Subscribe(x =>
                {
                    Console.WriteLine(x);
                    Console.WriteLine("Observable thread id: {0}",
                        Thread.CurrentThread.ManagedThreadId);
                });
        }

        static IObservable<int> Add2NumbersAsync(int a, int b)
        {
            //return Observable.Start(() => Add2Numbers(a, b));
            return Observable.Return(Add2Numbers(a, b));
        }
        static int Add2Numbers(int a, int b)
        {
            return a + b;
        }

        private static void testMaterialize()
        {
            Observable.Range(1, 3)
                .Materialize()
                .Dump("Materialize");
        }

        private static void testSelect1()
        {
            var letters = Observable.Range(0, 3)
                .Select(i => (char)(i + 65));
            var index = -1;
            var result = letters
                .Select(c =>
                {
                    index++;
                    return c;
                });
            result.Subscribe(
                c => Console.WriteLine("Received {0} at index {1}", c, index),
                () => Console.WriteLine("completed"));
        }

        private static void testSelectMany()
        {
            Observable.Return(5)
                .SelectMany(i => Observable.Range(1, i))
                .Dump("SelectMany");

            Observable.Range(1, 3)
                .SelectMany(i => Observable.Range(1, i))
                .Dump("SelectMany");

            Func<int, char> letter = i => (char)(i + 64);
            Observable.Range(1, 5) //Return(1)
                .SelectMany(i => Observable.Return(letter(i)))
                .Dump("SelectMany");
        }

        private static void testToEnumerable()
        {
            var period = TimeSpan.FromMilliseconds(200);
            var source = Observable.Timer(TimeSpan.Zero, period)
                .Take(5);
            var result = source.ToEnumerable();
            foreach (var value in result)
            {
                Console.WriteLine(value);
            }
            Console.WriteLine("done");
        }

        private static void testInterval()
        {
            Observable.Interval(TimeSpan.FromSeconds(1))
                .Take(3)
                .Timestamp()
                .Dump("TimeStamp");

            Observable.Interval(TimeSpan.FromSeconds(1))
                .Take(3)
                .TimeInterval()
                .Dump("TimeInterval");
        }

        private static void testSelect()
        {
            //var source = Observable.Range(0, 5);
            //source.Select(i=>i+3)
            //    .Dump("+3");

            //Observable.Range(1, 5)
            //    .Select(i => (char)(i + 64))
            //    .Dump("char");

            //Observable.Range(1, 5)
            //    .Select(
            //        i => new { number = i, Character = (char)(i + 64) })
            //    .Dump("anon");

            var query = from i in Observable.Range(0, 5)
                        select new { number = i, Character = (char)(i + 64) };
            query.Dump("anon");
        }

        private static void testAggregations()
        {
            var numbers = new Subject<int>();
            numbers.Dump("numbers");
            numbers.Min().Dump("Min");
            numbers.Average().Dump("Average");

            numbers.OnNext(1);
            numbers.OnNext(2);
            numbers.OnNext(3);
            numbers.OnCompleted();
        }

        private static void testCount()
        {
            var numbers = Observable.Range(0, 3);
            numbers.Dump("numbers");
            numbers.Count().Dump("count");
        }

        private static void testAll()
        {
            var subject = new Subject<int>();
            subject.Subscribe(Console.WriteLine, () => Console.WriteLine("Subject completed"));
            var all = subject.All(i => i < 5);
            all.Subscribe(b => Console.WriteLine("All values less than 5? {0}", b));
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(6);
            subject.OnNext(2);
            subject.OnNext(1);
            subject.OnCompleted();
        }

        private static void testContains()
        {
            var subject = new Subject<int>();
            subject.Subscribe(
            Console.WriteLine,
            () => Console.WriteLine("Subject completed"));

            var contains = subject.Contains(2);
            contains.Subscribe(
            b => Console.WriteLine("Contains the value 2? {0}", b),
            () => Console.WriteLine("contains completed"));

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(2);
            subject.OnCompleted();
        }

        private static void testSkipUntil()
        {
            var subject = new Subject<int>();
            var otherSubject = new Subject<Unit>();
            subject
            .SkipUntil(otherSubject)
            //.TakeWhile(otherSubject)
            .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            otherSubject.OnNext(Unit.Default);
            subject.OnNext(4);
            subject.OnNext(5);
            subject.OnNext(6);
            subject.OnNext(7);
            subject.OnCompleted();
        }

        private static void testSkipWhile()
        {
            var subject = new Subject<int>();
            subject.SkipWhile(i => i < 4)
                // takewhile is the oposed
                .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));

            subject.OnNext(3);
            subject.OnNext(3);
            subject.OnNext(5);
            subject.OnNext(2);
            subject.OnCompleted();
        }

        private static void testSkipAndTake()
        {
            Observable//.Range(0, 10)
                .Interval(TimeSpan.FromMilliseconds(500))
                //.Skip(3)
                .Take(3)
                .Subscribe(Console.WriteLine, () => Console.WriteLine("Completed"));
        }

        private static void testIgnoreElements()
        {
            var subject = new Subject<int>();

            var noElements = subject.IgnoreElements();
            /*
            // equivalent to
            subject.Where(value => false);
            // or functional style that implies that the value is ignored.
            subject.Where(_ => false);
            */

            subject.Subscribe(
                i => Console.WriteLine("subject.OnNext({0})", i),
                () => Console.WriteLine("subject.OnCompleted"));
            noElements.Subscribe(
                i => Console.WriteLine("noElement.OnNext({0})", i),
                () => Console.WriteLine("noElements.OnCompleted()"));

            subject.OnNext(1);
            subject.OnNext(23);
            subject.OnCompleted();
        }

        private static void testDistinct()
        {
            var subject = new Subject<int>();
            //var distinct = subject.Distinct();

            // useful in reducing noise
            var distinct = subject.DistinctUntilChanged();

            subject.Subscribe(
                i => Console.WriteLine("{0}", i),
                () => Console.WriteLine("subject.OnCompleted"));
            distinct.Subscribe(
                i => Console.WriteLine("distinct.OnNext({0})", i),
                () => Console.WriteLine("distinct.OnCompleted()"));

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(1);
            subject.OnNext(4);
            subject.OnCompleted();
        }

        private static void TestWhere()
        {
            var oddNumbers = Observable.Range(0, 10)
                .Where(i => i % 2 == 0)
                .Subscribe(
                    Console.WriteLine,
                    () => Console.WriteLine("completed"));
        }

        private static void TestObservableTimer()
        {
            //var interval = Observable.Interval(TimeSpan.FromMilliseconds(300)).Take(5);
            //var x = interval.Subscribe(
            //    Console.WriteLine,
            //    () => Console.WriteLine("completed"));

            var timer = Observable.Timer(TimeSpan.FromSeconds(2));
            timer.Subscribe(
                Console.WriteLine,
                () => Console.WriteLine("completed"));

            //var t1 = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        public static IObservable<long> Timer(TimeSpan dueTime)
        {
            return Observable.Generate(
            0l,
            i => i < 1,
            i => i + 1,
            i => i,
            i => dueTime);
        }
        public static IObservable<long> Timer(TimeSpan dueTime, TimeSpan period)
        {
            return Observable.Generate(
            0l,
            i => true,
            i => i + 1,
            i => i,
            i => i == 0 ? dueTime : period);
        }
        public static IObservable<long> Interval(TimeSpan period)
        {
            return Observable.Generate(
            0l,
            i => true,
            i => i + 1,
            i => i,
            i => period);
        }

        public static IObservable<int> Range(int start, int count)
        {
            var max = start + count;
            return Observable.Generate(
            start,
            value => value < max,
            value => value + 1,
            value => value);
        }

        private static void TestUnfold()
        {
            var naturalNumbers = Unfold(1, i => i + 1);
            Console.WriteLine("1st 10 Natural numbers");
            foreach (var naturalNumber in naturalNumbers.Take(10))
            {
                Console.WriteLine(naturalNumber);
            }
        }

        private static IEnumerable<T> Unfold<T>(T seed, Func<T, T> accumulator)
        {
            var nextValue = seed;
            while (true)
            {
                yield return nextValue;
                nextValue = accumulator(nextValue);
            }
        }

        public static void NonBlocking_event_driven()
        {
            var ob = Observable.Create<string>(
            observer =>
            {
                var timer = new System.Timers.Timer();
                timer.Interval = 1000;
                timer.Elapsed += (s, e) => observer.OnNext("tick");
                timer.Elapsed += OnTimerElapsed;
                timer.Start();
                //return Disposable.Empty;
                //return timer;
                return () =>
                {
                    timer.Elapsed -= OnTimerElapsed;
                    timer.Dispose();
                };
            });

            var subscription = ob.Subscribe(Console.WriteLine);
            Console.ReadLine();
            subscription.Dispose();
        }

        private static void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine(e.SignalTime);
        }

        private static IObservable<string> BlockingMethod()
        {
            var subject = new ReplaySubject<string>();
            subject.OnNext("a");
            subject.OnNext("b");
            subject.OnCompleted();
            Thread.Sleep(1000);
            return subject;
        }

        private static IObservable<string> NonBlocking()
        {
            return Observable.Create<string>(
            (IObserver<string> observer) =>
            {
                observer.OnNext("a");
                observer.OnNext("b");
                observer.OnCompleted();
                Thread.Sleep(1000);
                return Disposable.Create(() => Console.WriteLine("Observer has unsubscribed"));
                //or can return an Action like 
                //return () => Console.WriteLine("Observer has unsubscribed"); 
            });
        }

        private static void TestObservable()
        {
            var singleValue = Observable.Return("Value");
            // which could have also been simulated with a replay subject
            //var subject = new ReplaySubject<string>();
            //subject.OnNext("Value");
            //subject.OnCompleted();

            var empty = Observable.Empty<string>();
            // Behaviorally equivalent to
            //subject = new ReplaySubject<string>();
            //subject.OnCompleted();

            var never = Observable.Never<string>();
            //subject = new Subject<string>();

            var throws = Observable.Throw<string>(new Exception());
            // Behaviorally equivalent to
            //var subject = new ReplaySubject<string>();
            //subject.OnError(new Exception());
        }

        private static void TestFromEventPattern()
        {
            var txt = new TextBox();
            var form = new Form()
            {
                Controls = { txt }
            };

            var ts = Observable.FromEventPattern<EventArgs>(txt, "TextChanged");
            var res = (from e in ts
                       select ((TextBox)e.Sender).Text).DistinctUntilChanged()
                .Throttle(TimeSpan.FromSeconds(0.5));

            using (res.Subscribe(Console.WriteLine))
            {
                Application.Run(form);
            }
        }
    }


    public static class SampleExtentions
    {
        public static void Dump<T>(this IObservable<T> source, string name)
        {
            source.Subscribe(
            i => Console.WriteLine("{0}-->{1}", name, i),
            ex => Console.WriteLine("{0} failed-->{1}", name, ex.Message),
            () => Console.WriteLine("{0} completed", name));
        }
    }
}
