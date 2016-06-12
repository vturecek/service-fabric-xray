import {ViewModel} from './viewmodel';

export class Selectable extends ViewModel<string> {
    public constructor(
        public name: string,
        public selected: boolean)
    { super(name) }

    public copyFrom(other: Selectable) {
        this.name = other.name;
    }
}