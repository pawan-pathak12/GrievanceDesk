import { useEffect, useRef, useState } from 'react';
import { Pencil, Send, Trash2 } from 'lucide-react';
import { commentApi } from '../../api/commentApi';
import { useAuth } from '../../context/AuthContext';
import { useToast } from '../../context/ToastContext';
import { formatDateTime } from '../../utils/enums';
import TableActionButton from './TableActionButton';

const MAX_COMMENT_LENGTH = 1000;

export default function ComplaintDiscussion({ complaintId, isAdmin = false, embedded = false, autoFocus = false, autoScrollOnLoad = false }) {
  const [comments, setComments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [comment, setComment] = useState('');
  const [commentError, setCommentError] = useState('');
  const [posting, setPosting] = useState(false);
  const [editingId, setEditingId] = useState(null);
  const [editText, setEditText] = useState('');
  const [editError, setEditError] = useState('');
  const [savingId, setSavingId] = useState(null);
  const [commentToDelete, setCommentToDelete] = useState(null);
  const [deleting, setDeleting] = useState(false);
  const commentsEndRef = useRef(null);
  const commentInputRef = useRef(null);
  const { user } = useAuth();
  const toast = useToast();
  const currentUserId = user?.userId || user?.id;

  useEffect(() => {
    loadComments();
  }, [complaintId]);

  useEffect(() => {
    if (!autoFocus) return;
    window.requestAnimationFrame(() => commentInputRef.current?.focus());
  }, [autoFocus, complaintId]);

  useEffect(() => {
    if (autoScrollOnLoad && !loading && comments.length > 0) scrollToLatest();
  }, [autoScrollOnLoad, comments.length, loading]);

  async function loadComments() {
    setLoading(true);
    const result = await commentApi.getByComplaint(complaintId);
    if (result.isSuccess) {
      setComments(result.data || []);
    } else {
      toast.error(result.errors?.[0] || 'Failed to load discussion.');
    }
    setLoading(false);
  }

  function scrollToLatest() {
    window.requestAnimationFrame(() => {
      commentsEndRef.current?.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
    });
  }

  async function handlePost(event) {
    event.preventDefault();
    const value = comment.trim();
    if (!value) {
      setCommentError('Write a comment before posting.');
      return;
    }

    setPosting(true);
    setCommentError('');
    const result = await commentApi.create(complaintId, value);
    setPosting(false);

    if (result.isSuccess) {
      setComments((current) => [...current, result.data]);
      setComment('');
      toast.success('Comment added successfully.');
      scrollToLatest();
    } else {
      setCommentError(result.errors?.[0] || 'Unable to post comment.');
    }
  }

  function startEdit(item) {
    setEditingId(item.complaintCommentId);
    setEditText(item.comment);
    setEditError('');
  }

  function cancelEdit() {
    setEditingId(null);
    setEditText('');
    setEditError('');
  }

  async function saveEdit(commentId) {
    const value = editText.trim();
    if (!value) {
      setEditError('Comment cannot be empty.');
      return;
    }

    setSavingId(commentId);
    setEditError('');
    const result = await commentApi.update(commentId, value);
    setSavingId(null);

    if (result.isSuccess) {
      setComments((current) => current.map((item) => (
        item.complaintCommentId === commentId ? result.data : item
      )));
      cancelEdit();
      toast.success('Comment updated successfully.');
    } else {
      setEditError(result.errors?.[0] || 'Unable to update comment.');
    }
  }

  async function confirmDelete() {
    if (!commentToDelete) return;

    setDeleting(true);
    const result = isAdmin
      ? await commentApi.deleteAsAdmin(commentToDelete.complaintCommentId)
      : await commentApi.deleteOwn(commentToDelete.complaintCommentId);
    setDeleting(false);

    if (result.isSuccess) {
      setComments((current) => current.filter((item) => item.complaintCommentId !== commentToDelete.complaintCommentId));
      setCommentToDelete(null);
      toast.success('Comment deleted successfully.');
    } else {
      toast.error(result.errors?.[0] || 'Unable to delete comment.');
    }
  }

  return (
    <section className={embedded ? 'complaint-discussion complaint-discussion-embedded' : 'detail-card complaint-discussion-card'} aria-labelledby="discussion-title">
      <div className="discussion-heading">
        <div>
          <h2 id="discussion-title">Discussion</h2>
          <p>{comments.length} {comments.length === 1 ? 'comment' : 'comments'}</p>
        </div>
      </div>

      {loading ? (
        <div className="discussion-loading" aria-label="Loading discussion">
          <span />
          <span />
          <span />
        </div>
      ) : comments.length === 0 ? (
        <div className="discussion-empty-state">No discussions yet.</div>
      ) : (
        <div className="discussion-list">
          {comments.map((item) => {
            const isOwner = Number(item.userId) === Number(currentUserId);
            const isEditing = editingId === item.complaintCommentId;

            return (
              <article className="discussion-item" key={item.complaintCommentId}>
                <div className="discussion-item-header">
                  <div>
                    <strong>{item.userName}</strong>
                    <span className={`discussion-role discussion-role-${String(item.userRole).toLowerCase()}`}>{item.userRole}</span>
                  </div>
                  <div className="discussion-actions">
                    {isOwner && <TableActionButton label="Edit comment" icon={Pencil} onClick={() => startEdit(item)} disabled={isEditing || deleting} />}
                    {(isOwner || isAdmin) && <TableActionButton label="Delete comment" icon={Trash2} tone="danger" onClick={() => setCommentToDelete(item)} disabled={isEditing || deleting} />}
                  </div>
                </div>

                {isEditing ? (
                  <div className="discussion-edit-form">
                    <textarea value={editText} maxLength={MAX_COMMENT_LENGTH} onChange={(event) => setEditText(event.target.value)} disabled={savingId === item.complaintCommentId} />
                    {editError && <p className="discussion-error" role="alert">{editError}</p>}
                    <div className="discussion-edit-actions">
                      <button type="button" className="btn-cancel" onClick={cancelEdit} disabled={savingId === item.complaintCommentId}>Cancel</button>
                      <button type="button" className="btn-submit" onClick={() => saveEdit(item.complaintCommentId)} disabled={savingId === item.complaintCommentId}>
                        {savingId === item.complaintCommentId ? 'Saving...' : 'Save'}
                      </button>
                    </div>
                  </div>
                ) : (
                  <p className="discussion-item-message">{item.comment}</p>
                )}

                <div className="discussion-item-meta">
                  <time>{formatDateTime(item.createdAt)}</time>
                  {item.isEdited && <span>Edited</span>}
                </div>
              </article>
            );
          })}
          <div ref={commentsEndRef} />
        </div>
      )}

      <form className="discussion-compose" onSubmit={handlePost}>
        <label htmlFor={`complaint-comment-${complaintId}`}>Add a comment</label>
        <textarea
          id={`complaint-comment-${complaintId}`}
          ref={commentInputRef}
          value={comment}
          maxLength={MAX_COMMENT_LENGTH}
          placeholder="Write a comment..."
          disabled={posting}
          onChange={(event) => {
            setComment(event.target.value);
            if (commentError) setCommentError('');
          }}
        />
        <div className="discussion-compose-footer">
          <span>{comment.length}/{MAX_COMMENT_LENGTH}</span>
          <button type="submit" className="btn-submit" disabled={posting}>{posting ? 'Posting...' : <><Send size={16} aria-hidden="true" />Post Comment</>}</button>
        </div>
        {commentError && <p className="discussion-error" role="alert">{commentError}</p>}
      </form>

      {commentToDelete && (
        <div className="complaint-modal-backdrop" role="presentation" onMouseDown={(event) => event.target === event.currentTarget && !deleting && setCommentToDelete(null)}>
          <section className="complaint-delete-comment-dialog" role="dialog" aria-modal="true" aria-labelledby="delete-comment-title">
            <h2 id="delete-comment-title">Delete comment?</h2>
            <p>This comment will be removed from the discussion.</p>
            <div className="complaint-dialog-footer">
              <button type="button" className="btn-cancel" onClick={() => setCommentToDelete(null)} disabled={deleting}>Cancel</button>
              <button type="button" className="btn-danger" onClick={confirmDelete} disabled={deleting}>{deleting ? 'Deleting...' : 'Delete Comment'}</button>
            </div>
          </section>
        </div>
      )}
    </section>
  );
}
