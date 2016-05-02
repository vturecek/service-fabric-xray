import {ViewModel} from './viewmodel';

export abstract class DeployedEntityViewModel<T> extends ViewModel<T>
{
    public status: string;
    public healthState: string;

    public constructor(
        status: string,
        healthState: string,
        comparisonId: any)
    {
        super(comparisonId);

        this.status = status;
        this.healthState = healthState;
    }
}