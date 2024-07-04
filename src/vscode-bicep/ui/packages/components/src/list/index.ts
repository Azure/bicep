import { List as ListComponent } from './List';
import { ListItem } from './ListItem';

const List = Object.assign(ListComponent, { Item: ListItem });

export { List };
