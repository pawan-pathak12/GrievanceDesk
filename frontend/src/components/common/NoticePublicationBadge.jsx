export default function NoticePublicationBadge({ isPublished }) {
  return (
    <span className={`notice-publication-badge ${isPublished ? 'notice-published' : 'notice-draft'}`}>
      {isPublished ? 'Published' : 'Draft'}
    </span>
  );
}
