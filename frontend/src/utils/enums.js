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

// Reverse maps for display
export const CategoryLabels = Object.fromEntries(
  Object.entries(ComplaintCategory).map(([k, v]) => [v, k])
);

export const StatusLabels = {
  0: 'Pending',
  1: 'In Progress',
  2: 'Resolved',
  3: 'Irrelevant',
};

export const CategoryOptions = Object.entries(ComplaintCategory).map(([label, value]) => ({
  label,
  value,
}));
