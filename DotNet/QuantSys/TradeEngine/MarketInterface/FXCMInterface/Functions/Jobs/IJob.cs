namespace QuantSys.TradeEngine.MarketInterface.FXCMInterface.Functions.Jobs
{
    public interface IJob
    {
        void RunJob(FXSession fxsession);
        bool UpdateJob(object data);
        void FinishAndProcess();
    }
}