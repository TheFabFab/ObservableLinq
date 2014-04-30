# ObservableLinq


ObservableLinq is an open source, portable class library that can be easily deployed from [nuget] and used in .NET 4.5, Windows Phone 8 and Windows Store applications. ObservableLinq proposes to implement a subset of the [LINQ] operators on ObservableCollection, where source collection changes are automatically reflected in the result collection.

![a](https://raw.github.com/TheFabFab/ObservableLinq/master/img/basic_projection.gif)

## How would you use ObservableLinq?

### Project data between different domains

Larger applications are often architectured in multiple layers, across multiple tiers. The different layers can enrich data coming from another layer with new attributes, filters and sort criteria.

Consider an editable data grid that contains personal data in an MVVM application. The model class will contain records with first name, last name, etc. Your view model might wrap the model object and have additional attributes, like 'IsEditing' and 'IsChanged'. Your UI layer will transform the view model into rows in the datagrid.

Using ObservableLinq not only allows you to project your model into view models, but when your original collection changes, it will keep the view model collection up to date, without loosing the added data on the view model level.

### Change between different views of the same collection

ObservableLinq provides a special collection class, called ObserverCollection. This collection can be instructed to follow a different observable collection. However, ObserverCollection can change from one observed collection to another at any time. Items present in both will be retained and moved to their new position.

Let's consider our personal data view models from before. We can project that collection into one where we sort by first name and another one where we sort by last name. Maybe another one, where we filter out unchanged items. Then, we bind our datagrid to an observer collection that switches between these collections according to user input...

### Display changes visually to the user

The basic modification events on the ICollectionChanged interface can be translated into animations like in the figure above, which results in a user interface that is both appealing and gives plenty of visual clues to the user in order to correctly understand the underlying data manipulation.

## Current state

 - "Alpha" state
 - Functioning ObserverCollection
 - Select, Distinct, Where, OrderBy operators *mostly* ready

## License

[Apache License Version 2.0]

## Contributions

Pull requests are welcome. In order to be accepted, the functionality has to be unit tested against the static Linq operators.

[nuget]: https://www.nuget.org/
[LINQ]: http://msdn.microsoft.com/en-us/library/bb397926.aspx
[Apache License Version 2.0]:https://github.com/TheFabFab/ObservableLinq/blob/master/License.txt
