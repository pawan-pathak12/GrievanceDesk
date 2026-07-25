export default function TableActionButton({ label, icon: Icon, tone = 'primary', className = '', ...props }) {
  return (
    <button
      type="button"
      className={`table-action-icon table-action-${tone} ${className}`}
      title={label}
      aria-label={label}
      {...props}
    >
      <Icon size={17} strokeWidth={2.2} aria-hidden="true" />
    </button>
  );
}
