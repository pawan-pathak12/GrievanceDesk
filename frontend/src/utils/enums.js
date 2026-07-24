export const UserRole = { Admin: 0, User: 1 };

export const ComplaintCategory = {
  General: 0,
  Infrastructure: 1,
  Service: 2,
  Staff: 3,
  Safety: 4,
  Cleanliness: 5,
  Other: 6,
};

export const ComplaintStatus = {
  Pending: 0,
  InProgress: 1,
  Resolved: 2,
  Irrelevant: 3,
};

export const StatusLabels = {
  0: 'Pending',
  1: 'In Progress',
  2: 'Resolved',
  3: 'Irrelevant',
};

export const CategoryLabels = Object.fromEntries(
  Object.entries(ComplaintCategory).map(([key, value]) => [value, key])
);

export const StatusNames = {
  Pending: 0,
  InProgress: 1,
  Resolved: 2,
  Irrelevant: 3,
};

export const CategoryOptions = Object.entries(ComplaintCategory).map(([label, value]) => ({
  label,
  value,
}));

export const StatusOptions = Object.entries(ComplaintStatus).map(([label, value]) => ({
  label: StatusLabels[value] || label,
  value,
  name: label,
}));

export function normalizeCategory(category) {
  if (typeof category === 'number') return category;
  if (typeof category === 'string') {
    const compact = category.replace(/\s+/g, '');
    return ComplaintCategory[compact] ?? Number(category);
  }
  return ComplaintCategory.General;
}

export function normalizeStatus(status) {
  if (typeof status === 'number') return status;
  if (typeof status === 'string') {
    const compact = status.replace(/\s+/g, '');
    return StatusNames[compact] ?? Number(status);
  }
  return ComplaintStatus.Pending;
}

export function formatCategory(category) {
  const normalized = normalizeCategory(category);
  return CategoryLabels[normalized] || String(category || 'General');
}

export function formatStatus(status) {
  const normalized = normalizeStatus(status);
  return StatusLabels[normalized] || String(status || 'Unknown');
}

export function formatDate(value, options) {
  if (!value) return 'Not available';
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return 'Not available';
  return date.toLocaleDateString('en-US', options || {
    month: 'short',
    day: 'numeric',
    year: 'numeric',
  });
}

export function formatDateTime(value) {
  if (!value) return 'Not available';
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return 'Not available';
  return date.toLocaleString('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  });
}
