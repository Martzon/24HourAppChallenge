import { User } from "./user.model";

export class UserPaginate {
  hasNextPage?: boolean;
  hasPreviousPage?: boolean;
  items?: User;
  pageNumber?: number;
  totalCount?: number;
  totalPages?: number;
}
