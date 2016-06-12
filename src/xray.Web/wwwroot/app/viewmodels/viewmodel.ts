export abstract class ViewModel<T>
{
    public constructor(protected comparisonId: any) { }

    public abstract copyFrom(other: ViewModel<T>) : void;

    public equals(other: ViewModel<T>): boolean {
        return !other || (this.comparisonId == other.comparisonId);
    }
}