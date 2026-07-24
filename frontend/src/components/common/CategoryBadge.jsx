import { formatCategory } from '../../utils/enums';

export default function CategoryBadge({ category }) {
  const label = formatCategory(category);
  return <span className="badge badge-category">{label}</span>;
}
