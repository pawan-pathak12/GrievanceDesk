import { CategoryLabels } from '../../utils/enums';

export default function CategoryBadge({ category }) {
  const label = CategoryLabels[category] || 'General';
  return <span className="badge badge-category">{label}</span>;
}
