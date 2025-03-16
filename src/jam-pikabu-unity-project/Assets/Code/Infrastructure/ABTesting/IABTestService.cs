namespace Code.Infrastructure.ABTesting
{
    public interface IABTestService
    {
        ExperimentValueTypeId GetExperimentValue(ExperimentTagTypeId tag);
    }
}