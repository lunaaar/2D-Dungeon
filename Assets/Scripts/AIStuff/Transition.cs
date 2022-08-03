public class Transition
{
    public delegate bool ConditionCallback();
    public ConditionCallback condition;
    public State target;
}