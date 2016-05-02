import {ViewModel} from './viewmodel';

export class List
{
    public static updateList<T>(oldList: ViewModel<T>[], newList: ViewModel<T>[]) {
        if (!newList) {
            console.log("nothing new");
            return;
        }

        // remove items that aren't in the new list
        for (let i = 0; i < oldList.length; ++i) {
            let oldItem = oldList[i];

            if (!newList.find(x => x.equals(oldItem))) {
                oldList.splice(i, 1);
            }
        }

        // add or update items from then new list
        for (let i = 0; i < newList.length; ++i) {
            let newItem = newList[i];

            let oldItem: ViewModel<T> = oldList.find(x => x.equals(newItem));

            if (!oldItem) {
                oldList.push(newItem);
            }
            else {
                oldItem.copyFrom(newItem);
            }
        }
    }
}